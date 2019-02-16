using System;
using System.Collections.Generic;
using System.Linq;
using core.Extensions.Base;
using Microsoft.Extensions.DependencyInjection;

namespace core.Extensions.Data.EF.SqlServer
{
    public class Options : IOptions
    {
        public IEnumerable<core.Extensions.Data.DbConnection> Connections { get; set; }
        public ServiceLifetime ServiceLifetime { get; set; } = ServiceLifetime.Scoped;
        public MergeConfig Merge { get; set; }
        public class MergeConfig: EFCore.BulkExtensions.BulkConfig
        {
            public new bool? UseTempDB { get; set; } = true;
            public new List<string> UpdateByProperties = new[] { "Id" }.ToList();
            public new int? BulkCopyTimeout { get; set; } = 180;
        }
    }
}
