using System;
using System.Collections.Generic;

namespace Ws.Core.Extensions.Data
{
    public class DbConnection
    {
        public string Name { get; set; } = "default";
        public string ConnectionString { get; set; }
        public string Database { get; set; }
        public ServiceResolverCriteria ServiceResolver { get; set; } = new ServiceResolverCriteria();
        public class ServiceResolverCriteria
        {
            public ServiceResolverSelector Include { get; set; } = new ServiceResolverSelector();
            public ServiceResolverSelector Exclude { get; set; } = new ServiceResolverSelector();
        }
        public class ServiceResolverSelector
        {
            public string[] Assembly { get; set; } = Array.Empty<string>();
            public string[] Namespace { get; set; } = Array.Empty<string>();
            public string[] FullName { get; set; } = Array.Empty<string>();
        }
    }
}
