using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ws.Core.Extensions.HealthCheck.Checks.AppLog;

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
        public IEnumerable<LogRuleSelector> Selectors { get; set; }
    }
    public class LogRuleSelector
    {
        public LogRuleSelectorMatch Logger { get; set; }
        public LogRuleSelectorMatch Message { get; set; }

        public class LogRuleSelectorMatch
        {
            public string[] List { get; set; }
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
        public int TruncateLengthAt { get; set; } = 255;
        /// <summary>
        /// Levenshtein distance factor (0-100 percent) used to aggregate messages
        /// </summary>
        public int MaxLevenshteinDistanceFactor { get; set; } = 50;
    }
    public class LogLevelMinCounter
    {
        public int MinEntry { get; set; }
        public HealthStatus HealthStatus { get; set; }
    }
    public class LogHealthStatusChecker
    {
        public LogLevel Level { get; set; }
        public IEnumerable<LogLevelMinCounter> MinCounters { get; set; }
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
        public string AppLogService { get; set; }
        /// <summary>
        /// Parse last logs by criteria
        /// </summary>
        public TakeLastLogCriteria TakeLastLog { get; set; } = new();
        public IEnumerable<LogHealthStatusChecker> HealthStatusCheckers { get; set; }
        /// <summary>
        /// Ignore log based on logger/message
        /// </summary>
        public IEnumerable<LogRule> LogIgnoreRoles { get; set; }
        public LogMessageAggregate LogMessageAggregate { get; set; } = new LogMessageAggregate();
    }
}

namespace Ws.Core.Extensions.HealthCheck
{
    public partial class Options
    {
        public partial class CheckEntries
        {
            public Ws.Core.Extensions.HealthCheck.Checks.AppLog.Options AppLog { get; set; } 
        }
    }
}
