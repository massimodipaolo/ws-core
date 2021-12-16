using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ws.Core.Extensions.HealthCheck.Checks.AppLog
{
    public interface IAppLogService
    {
        IQueryable<ILog> List { get; }
    }
}
