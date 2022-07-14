using System;
using System.Collections.Generic;
using Ws.Core.Extensions.Base;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;

namespace Ws.Core.Extensions.Data.EF.SQLite
{
    public class Options : IOptions
    {
        public IEnumerable<Extensions.Data.DbConnection>? Connections { get; set; }
        [DefaultValue(ServiceLifetime.Scoped)]
        public ServiceLifetime ServiceLifetime { get; set; } = ServiceLifetime.Scoped;
    }
}