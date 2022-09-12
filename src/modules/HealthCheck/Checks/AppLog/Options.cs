using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.ComponentModel;

namespace Ws.Core.Extensions.HealthCheck.Checks.AppLog
{
    public enum LogLevel
    {
        Trace,
        Debug,
        Info,
        Warn,
        Error,
        Fatal
    }
    public class LogRule
    {
        public LogLevel Level { get; set; }
        // Selector roles to ignore log for this severity level
        [Description("Selector roles to ignore log for this severity level")]
        public IEnumerable<LogRuleSelector> Selectors { get; set; } = Array.Empty<LogRuleSelector>();
    }
    [Description("Select log entry filtering logger AND message constraints")]
    public class LogRuleSelector
    {
        public LogRuleSelectorMatch? Logger { get; set; }
        public LogRuleSelectorMatch? Message { get; set; }

        public class LogRuleSelectorMatch
        {
            public string[] List { get; set; } = Array.Empty<string>();
            [Description("Performance order: equalTo > startWith > contains > regEx")]
            [DefaultValue(LogRuleSelectorMatchType.EqualTo)]
            public LogRuleSelectorMatchType Role { get; set; } = LogRuleSelectorMatchType.EqualTo;            
        }
        public enum LogRuleSelectorMatchType
        {
            EqualTo,
            StartWith,
            Contains,
            RegEx
        }
    }
    public class LogMessageAggregate
    {
        /// <summary>
        /// Message max chars
        /// </summary>
        [Description("Message max chars. 0 to skip message aggregation. Range: 0 - 4000")]
        [DefaultValue(255)]
        public int TruncateLengthAt { get; set; } = 255;
        /// <summary>
        /// Levenshtein distance factor (0-100 percent) used to aggregate messages
        /// </summary>
        [Description("Levenshtein max distance factor (0-100 percent) used to aggregate messages. 0 = same string.")]
        [DefaultValue(50)]
        public int MaxLevenshteinDistanceFactor { get; set; } = 50;
    }
    public class LogLevelMinCounter
    {
        [Description("Min occurrance to set a new healthStatus, i.e. above 1000 Warn set healthStatus Degraded")]
        public int MinEntry { get; set; }
        public HealthStatus HealthStatus { get; set; } = Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy;
    }
    public class LogHealthStatusChecker
    {
        public LogLevel? Level { get; set; }

        public IEnumerable<LogLevelMinCounter> MinCounters { get; set; } = Array.Empty<LogLevelMinCounter>();
    }
    public class TakeLastLogCriteria
    {
        public double Value { get; set; } = 1;
        public Criterias Criteria { get; set; } = Criterias.FromHours;
        public enum Criterias
        {
            Top,
            FromHours
        }
    }
    public class Options: Extensions.HealthCheck.Options.HealthResult
    {
        public Options() { }

        /// <summary>
        /// IAppLogService concrete implementation class
        /// </summary>
        [Description("IAppLogService concrete implementation class, i.e. \"MyNamespace.MyServices.MyAppLogService, MyAssembly\"\r\nOtherwise autodiscover will used, or add a transient service in Startup class before extensions discovery.")]
        public string? AppLogService { get; set; }
        /// <summary>
        /// Parse last logs by criteria
        /// </summary>
        [Description("Parse last logs by criteria")]
        public TakeLastLogCriteria TakeLastLog { get; set; } = new();
        public IEnumerable<LogHealthStatusChecker>? HealthStatusCheckers { get; set; }
        /// <summary>
        /// Ignore log based on logger/message
        /// </summary>
        public IEnumerable<LogRule>? LogIgnoreRoles { get; set; }
        public LogMessageAggregate LogMessageAggregate { get; set; } = new ();
    }
}

namespace Ws.Core.Extensions.HealthCheck
{
    public partial class Options
    {
        public partial class CheckEntries
        {
            public Ws.Core.Extensions.HealthCheck.Checks.AppLog.Options? AppLog { get; set; } 
        }
    }
}
