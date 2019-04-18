using System;
using System.Collections.Generic;
using Ws.Core.Extensions.Base;
using Microsoft.Extensions.DependencyInjection;

namespace Ws.Core.Extensions.Data.EF.MySql
{
    public class Options : IOptions
    {
        public IEnumerable<Extensions.Data.DbConnection> Connections { get; set; }
        public ServiceLifetime ServiceLifetime { get; set; } = ServiceLifetime.Scoped;
    }
}