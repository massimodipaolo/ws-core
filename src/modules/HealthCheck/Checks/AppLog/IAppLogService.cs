namespace Ws.Core.Extensions.HealthCheck.Checks.AppLog;

public interface IAppLogService
{
    IQueryable<ILog> List { get; }
}
