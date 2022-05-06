using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Ws.Core.Extensions.HealthCheck.Checks.AppLog
{
    public class HealthCheck : IHealthCheck
    {
        private readonly Options _options;
        private readonly IAppLogService _service;

        public HealthCheck(Options options, Ws.Core.Extensions.HealthCheck.Checks.AppLog.IAppLogService service)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            // Take last logs
            var _logs = _service.List;
            IQueryable<ILog> items = (_options.TakeLastLog.Criteria switch
            {
                TakeLastLogCriteria.Criterias.Top => _logs.OrderByDescending(_ => _.CreatedAt).Take((int)_options.TakeLastLog.Value),
                _ => _logs.Where(_ => _.CreatedAt > DateTime.UtcNow.AddHours(-_options.TakeLastLog.Value)),
            }).ToList().AsQueryable();

            // MessageAggregate Options
            int messageAggregateTruncateLengthAt = _options.LogMessageAggregate.TruncateLengthAt;
            int messageAggregateMaxDistanceFactor = _options.LogMessageAggregate.MaxLevenshteinDistanceFactor;
            Func<Dictionary<string, int>, string, Dictionary<string, int>> messageAggregate = (list, next) =>
            {
                // init
                if (!list.Any())
                    list[next] = 1;
                else
                {
                    // add/update
                    var prev = list.Last().Key;
                    int distance = Fastenshtein.Levenshtein.Distance(prev,next);
                    if (distance <= (int)(prev.Length * messageAggregateMaxDistanceFactor / 100.0))
                        list[prev] += 1;
                    else
                        list[next] = 1;
                }
                return list;
            };

            Func<ILog, string, string> matchSelectorMatchGetProperty = (_, property) => ((property == "Logger" ? _.Logger : _.Message) ?? "");
            Expression<Func<ILog, bool>> matchSelectorMatchExpression(LogRuleSelector.LogRuleSelectorMatch selectorMatch, string property)
            {
                return selectorMatch?.Role switch
                {
                    // EF compliant: pure expression with no func, query translated
                    LogRuleSelector.LogRuleSelectorMatchType.EqualTo =>
                        (_ => selectorMatch == null || selectorMatch.List.Any(__ => ((property == "Logger" ? _.Logger : _.Message) ?? "") == __)),
                    // EF non-compliant cases, IQueryable > ToList
                    LogRuleSelector.LogRuleSelectorMatchType.StartWith =>
                        (_ => selectorMatch == null || selectorMatch.List.Any(__ => matchSelectorMatchGetProperty(_, property).StartsWith(__))),
                    LogRuleSelector.LogRuleSelectorMatchType.Contains =>
                        (_ => selectorMatch == null || selectorMatch.List.Any(__ => matchSelectorMatchGetProperty(_, property).Contains(__))),
                    LogRuleSelector.LogRuleSelectorMatchType.RegEx =>
                        (_ => selectorMatch == null || selectorMatch.List.Any(__ => new Regex(__).IsMatch(matchSelectorMatchGetProperty(_, property)))),
                    _ => _ => true
                };
            }

            // result
            HealthStatus status = HealthStatus.Healthy;
            string description = "";
            Dictionary<string, object> data = new();
            Exception exception = null;

            try
            {
                // Ignore logs
                if (_options.LogIgnoreRoles?.Any() == true)
                {
                    // EF compliant
                    if (_options.LogIgnoreRoles.Any(_ => _.Selectors?
                            .Any(__ =>
                                __.Logger?.Role > LogRuleSelector.LogRuleSelectorMatchType.EqualTo ||
                                __.Message?.Role > LogRuleSelector.LogRuleSelectorMatchType.EqualTo
                            ) == true)
                        )
                        items = items.ToList().AsQueryable();

                    foreach (var role in _options.LogIgnoreRoles?.Where(_ => _.Selectors?.Any() == true))
                        foreach (var selector in role.Selectors?
                            .Where(_ =>
                                (_.Logger != null && _.Logger.List.Any()) || (_.Message != null && _.Message.List.Any())
                                )
                            )
                        {
                            var _selected = items
                                .Where(_ => _.Level == role.Level)
                                .Where(matchSelectorMatchExpression(selector.Logger, nameof(selector.Logger)))
                                .Where(matchSelectorMatchExpression(selector.Message, nameof(selector.Message)))
                                ;

                            items = items
                                .Except(_selected);
                        }
                }

                // Valuate status by checkers
                foreach (var checker in _options.HealthStatusCheckers?.OrderBy(_ => _.Level))
                {
                    if (!data.ContainsKey($"{checker.Level}"))
                    {
                        var byLevel = items.Where(_ => _.Level == checker.Level);
                        var count = byLevel.Count();
                        HealthCheckData _data = new() { Count = count };

                        _data.Patterns = byLevel
                            // EF compliant
                            .ToList()
                            .GroupBy(_ => _.Logger)
                            .Select(_ => new HealthCheckData.HealthCheckLoggerPattern()
                            {
                                Logger = _.Key,
                                Count = _.Count(),
                                Messages = _
                                    .Select(_ => _.Message[..Math.Min(messageAggregateTruncateLengthAt, _.Message.Length)])
                                    .OrderBy(_ => _)
                                    .Aggregate(new Dictionary<string, int>(), (list, next) => messageAggregate(list, next))
                                    .Select(_ => new HealthCheckData.HealthCheckMessagePattern() { Message = _.Key, Count = _.Value })
                                    .OrderByDescending(_ => _.Count).ThenBy(_ => _.Message)
                            })
                            .OrderByDescending(_ => _.Count).ThenBy(_ => _.Logger);

                        data.Add($"{checker.Level}", _data);

                        foreach (var counters in checker.MinCounters.Where(_ => count >= _.MinEntry && status > _.HealthStatus))
                        {
                            // concat description
                            description += $"Level {checker.Level} check: {count}/{counters.MinEntry} => status {counters.HealthStatus} {Environment.NewLine}";
                            // set new status
                            status = counters.HealthStatus;
                        }
                    }
                }
                
            } catch (Exception ex)
            {
                status = HealthStatus.Unhealthy;
                description = ex.Message;
                exception = new Exception($"{ex.Message} {ex.InnerException} {ex.StackTrace}");
            }

            HealthCheckResult result = new(status, description, exception, data);
            return Task.FromResult(result);
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

    }
}
