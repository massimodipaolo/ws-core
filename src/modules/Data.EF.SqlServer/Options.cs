using System;
using System.Collections.Generic;
using core.Extensions.Base;
using Microsoft.Extensions.DependencyInjection;

namespace core.Extensions.Data.EF.SqlServer
{
    public class Options : IOptions
    {
        public IEnumerable<core.Extensions.Data.DbConnection> Connections { get; set; }
        public ServiceLifetime ServiceLifetime { get; set; } = ServiceLifetime.Scoped;
    }
}
