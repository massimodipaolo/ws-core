using Ws.Core.Extensions.Data;
using Ws.Core.Extensions.HealthCheck.Checks.AppLog;
using x.core.Models;
using Xunit;
using Xunit.Abstractions;
using AppLog = Ws.Core.Extensions.HealthCheck.Checks.AppLog;

namespace x.core;

public class HealthCheck : BaseTest
{
    public HealthCheck(Program factory, ITestOutputHelper output) : base(factory, output) { }

    [Theory]
    [InlineData("/healthz")]
    [InlineData("/healthz/checks")]
    [InlineData("/healthchecks-ui")]
    [InlineData("/healthchecks-webhooks")]
    public async Task Get_Endpoints(string url) => await Get_EndpointsReturnSuccess(url);

    [Theory]
    [InlineData(typeof(Ws.Core.Extensions.HealthCheck.Checks.AppLog.IAppLogService), typeof(x.core.HealthCheckAppLogService))]
    public void AppLog_AutoDiscover(Type Tinterface, Type ExpectedTimplementation) => base.Check_ServiceImplementation(Tinterface, ExpectedTimplementation);
}

public class HealthCheckAppLogService : IAppLogService
{
    private readonly IRepository<Log, int> _repo;
    public HealthCheckAppLogService(IRepository<Log, int> repo)
    {
        _repo = repo;
    }

    public IQueryable<ILog> List => _repo.List.Where(_ => new AppLog.LogLevel[] { AppLog.LogLevel.Warn, AppLog.LogLevel.Error, AppLog.LogLevel.Fatal }.Contains(_.Level));
}

public class HealthCheckAppLogService<TRepo> : HealthCheckAppLogService where TRepo : IRepository<Log, int>
{
    public HealthCheckAppLogService(TRepo repo): base(repo) {}
}

public record Log : IRecord, ILog, IAppTracked, IEntity<int>
{
    public int Id { get; set; }
    public string MachineName { get; set; } = "";
    public string Logger { get; set; } = "";
    public AppLog.LogLevel Level { get; set; }
    public string Message { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
