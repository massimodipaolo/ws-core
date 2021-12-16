using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ws.Core.Extensions.Data;
using Ws.Core.Extensions;
using Ws.Core.Extensions.HealthCheck.Checks.AppLog;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace web.Code
{

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
