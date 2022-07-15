namespace Ws.Core.Extensions.HealthCheck.Checks.AppLog;

public interface ILog
{
    string MachineName { get; set; }
    string Logger { get; set; }
    LogLevel Level { get; set; }
    string Message { get; set; }
    DateTime CreatedAt { get; set; }
}
