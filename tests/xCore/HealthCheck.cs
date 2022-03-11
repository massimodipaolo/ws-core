using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ws.Core.Extensions.Data;
using Ws.Core.Extensions;
using Ws.Core.Extensions.HealthCheck.Checks.AppLog;
using Xunit.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using xCore.Extensions;
using System.Text.Json;

namespace xCore
{
    public class HealthCheck : BaseTest
    {
        public HealthCheck(Program<Startup> factory, ITestOutputHelper output) : base(factory, output) { }

        [Theory]
        [InlineData("/healthz")]
        [InlineData("/healthz/checks")]
        [InlineData("/healthchecks-api")]
        [InlineData("/healthchecks-ui")]
        [InlineData("/healthchecks-webhooks")]
        public async Task Get_Endpoints(string url) => await Get_EndpointsReturnSuccess(url);

        [Theory]
        [InlineData(typeof(Ws.Core.Extensions.HealthCheck.Checks.AppLog.IAppLogService), typeof(xCore.HealthCheckAppLogService))]
        public void AppLog_AutoDiscover(Type Tinterface, Type ExpectedTimplementation) => base.Check_ServiceImplementation(Tinterface, ExpectedTimplementation);
    }

    public class HealthCheckAppLogService : IAppLogService
    {
        private readonly IRepository<Log, int> _repo;

        public HealthCheckAppLogService(IRepository<Log, int> repo)
        {
            _repo = repo;
        }

        public IQueryable<ILog> List => _repo.List.Where(_ => new LogLevel[] {LogLevel.Warn, LogLevel.Error, LogLevel.Fatal}.Contains(_.Level));
    }

    public class Log : ILog, IEntity<int>
    {
        public int Id { get; set; }
        public string MachineName { get; set; }
        public string Logger { get; set; }
        public LogLevel Level { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
