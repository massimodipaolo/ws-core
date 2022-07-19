using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Ws.Core.Extensions.HealthCheck.Checks.AppLog;

public class HealthCheck : IHealthCheck
{
    private readonly Options _options;
    private readonly IAppLogService _service;
    // MessageAggregate Options
    private int _messageAggregateTruncateLengthAt => _options?.LogMessageAggregate?.TruncateLengthAt ?? 255;
    private int _messageAggregateMaxDistanceFactor => _options?.LogMessageAggregate?.MaxLevenshteinDistanceFactor ?? 50;
    public HealthCheck(Options options, Ws.Core.Extensions.HealthCheck.Checks.AppLog.IAppLogService service)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {            
        IQueryable<ILog> items = _takeLastLog();
        HealthCheckResultDetail resultDetail = new ();

        try
        {
            _filterIgnoreLog(ref items);
            _setResultByCheckers(resultDetail, ref items);
        } catch (Exception ex)
        {
            resultDetail.Status = HealthStatus.Unhealthy;
            resultDetail.Description.Append(ex.Message);
            resultDetail.Exception = new Exception($"{ex.Message} {ex.InnerException} {ex.StackTrace}");
        }

        HealthCheckResult result = new(resultDetail.Status, resultDetail.Description.ToString(), resultDetail.Exception, resultDetail.Data);
        return Task.FromResult(result);
    }

    /// <summary>
    /// // Take last logs: Top N/by Date
    /// </summary>
    /// <returns></returns>
    private IQueryable<ILog> _takeLastLog()
    {
        var _logs = _service.List;
#pragma warning disable S2971 // "IEnumerable" LINQs should be simplified: force ToList() to execute query due performance reason (disable S2971)
        return (_options.TakeLastLog.Criteria switch
        {
            TakeLastLogCriteria.Criterias.Top => _logs.OrderByDescending(_ => _.CreatedAt).Take((int)_options.TakeLastLog.Value),
            _ => _logs.Where(_ => _.CreatedAt > DateTime.UtcNow.AddHours(-_options.TakeLastLog.Value)),
        })
        .ToList()
        .AsQueryable();
#pragma warning restore S2971 // "IEnumerable" LINQs should be simplified
    }

    /// <summary>
    /// Filter items by Ignore rules
    /// </summary>
    /// <param name="items"></param>
    private void _filterIgnoreLog(ref IQueryable<ILog> items)
    {
        if (_options.LogIgnoreRoles?.Any() == true)
        {             

            // EF compliant: force ToList() to avoid tsql tranlation (disable S2971)
            if (_options.LogIgnoreRoles.Any(_ => _.Selectors?
                    .Any(__ =>
                        __.Logger?.Role > LogRuleSelector.LogRuleSelectorMatchType.EqualTo ||
                        __.Message?.Role > LogRuleSelector.LogRuleSelectorMatchType.EqualTo
                    ) == true)
                )
#pragma warning disable S2971 // "IEnumerable" LINQs should be simplified
                items = items
                    .ToList()
                    .AsQueryable();
#pragma warning restore S2971 // "IEnumerable" LINQs should be simplified

            foreach (var role in _options.LogIgnoreRoles?.Where(_ => _.Selectors?.Any() == true))
                foreach (var selector in role.Selectors?
                    .Where(_ =>
                        (_.Logger != null && _.Logger.List.Any()) || (_.Message != null && _.Message.List.Any())
                        )
                    )
                {
                    var _selected = items
                        .Where(_ => _.Level == role.Level)
                        .Where(_matchSelectorMatchExpression(selector.Logger, nameof(selector.Logger)))
                        .Where(_matchSelectorMatchExpression(selector.Message, nameof(selector.Message)))
                        .AsEnumerable()
                        ;

                    items = items
                        .Except(_selected);
                }
        }
    }

    private static Expression<Func<ILog, bool>> _matchSelectorMatchExpression(LogRuleSelector.LogRuleSelectorMatch selectorMatch, string property)
    {
        Func<ILog, string, string> _matchSelectorMatchGetProperty = (_, property) => ((property.Equals("logger", StringComparison.OrdinalIgnoreCase) ? _.Logger : _.Message) ?? "");
        return selectorMatch?.Role switch
        {
            // EF compliant: pure expression with no func, query translated. Trade-off: duplicate _matchSelectorMatchGetProperty
            LogRuleSelector.LogRuleSelectorMatchType.EqualTo =>
                (_ => selectorMatch == null || selectorMatch.List.Any(__ => ((property.Equals("logger", StringComparison.OrdinalIgnoreCase) ? _.Logger : _.Message) ?? "") == __)),
            // EF non-compliant cases, IQueryable > ToList
            LogRuleSelector.LogRuleSelectorMatchType.StartWith =>
                (_ => selectorMatch == null || selectorMatch.List.Any(__ => _matchSelectorMatchGetProperty(_, property).StartsWith(__))),
            LogRuleSelector.LogRuleSelectorMatchType.Contains =>
                (_ => selectorMatch == null || selectorMatch.List.Any(__ => _matchSelectorMatchGetProperty(_, property).Contains(__))),
            LogRuleSelector.LogRuleSelectorMatchType.RegEx =>
                (_ => selectorMatch == null || selectorMatch.List.Any(__ => new Regex(__).IsMatch(_matchSelectorMatchGetProperty(_, property)))),
            _ => _ => true
        };
    }

    /// <summary>
    /// Valuate status by checkers
    /// </summary>
    /// <param name="result"></param>
    /// <param name="items"></param>
    private void _setResultByCheckers(HealthCheckResultDetail result, ref IQueryable<ILog> items)
    {            
        foreach (var checker in _options.HealthStatusCheckers?.OrderBy(_ => _.Level))
        {
            if (!result.Data.ContainsKey($"{checker.Level}"))
            {
                var byLevel = items.Where(_ => _.Level == checker.Level);
                var count = byLevel.Count();
                HealthCheckData _data = new()
                {
                    Count = count,
                    Patterns = byLevel
                    // EF compliant
                    .AsEnumerable()
                    .GroupBy(_ => _.Logger)
                    .Select(_ => new HealthCheckData.HealthCheckLoggerPattern()
                    {
                        Logger = _.Key,
                        Count = _.Count(),
                        Messages = _
                            .Select(_ => _.Message[..Math.Min(_messageAggregateTruncateLengthAt, _.Message.Length)])
                            .OrderBy(_ => _)
                            .Aggregate(new Dictionary<string, int>(), (list, next) => _messageAggregate(list, next))
                            .Select(_ => new HealthCheckData.HealthCheckMessagePattern() { Message = _.Key, Count = _.Value })
                            .OrderByDescending(_ => _.Count).ThenBy(_ => _.Message)
                    })
                    .OrderByDescending(_ => _.Count).ThenBy(_ => _.Logger)
                };

                result.Data.Add($"{checker.Level}", _data);

                foreach (var counters in checker.MinCounters.Where(_ => count >= _.MinEntry && result.Status > _.HealthStatus))
                {
                    // concat description
                    result.Description.Append($"Level {checker.Level} check: {count}/{counters.MinEntry} => status {counters.HealthStatus} {Environment.NewLine}");
                    // set new status
                    result.Status = counters.HealthStatus;
                }
            }
        }
    }
    private Dictionary<string, int> _messageAggregate(Dictionary<string, int> list, string next)
    {
        // init
        if (!list.Any())
            list[next] = 1;
        else
        {
            // add/update
            var prev = list.Last().Key;
            int distance = Fastenshtein.Levenshtein.Distance(prev, next);
            if (distance <= (int)(prev.Length * _messageAggregateMaxDistanceFactor / 100.0))
                list[prev] += 1;
            else
                list[next] = 1;
        }
        return list;
    }

    public class HealthCheckData
    {
        public int Count { get; set; }
        public IEnumerable<HealthCheckLoggerPattern> Patterns { get; set; }
        public class HealthCheckLoggerPattern
        {
            public int Count { get; set; }
            public string Logger { get; set; }
            public IEnumerable<HealthCheckMessagePattern> Messages { get; set; }
        }

        public class HealthCheckMessagePattern
        {
            public int Count { get; set; }
            public string Message { get; set; }
        }
    }

    public record HealthCheckResultDetail
    {
        public HealthStatus Status { get; set; } = HealthStatus.Healthy;
        public System.Text.StringBuilder Description { get; set; }  = new();
        public Dictionary<string, object> Data { get; set; } = new();
        public Exception Exception { get; set; } = null;
    }

}
