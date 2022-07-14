using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;

namespace Ws.Core.Extensions.Data.Cache.SqlServer
{
    public class SqlCache : DistributedCache
    {
        public SqlCache(IOptions<Microsoft.Extensions.Caching.SqlServer.SqlServerCacheOptions> options)
            : base(new Microsoft.Extensions.Caching.SqlServer.SqlServerCache(options)) { }
    }
    public class SqlCache<T> : DistributedCache<T> where T : class
    {
        public SqlCache(IOptions<Microsoft.Extensions.Caching.SqlServer.SqlServerCacheOptions> options)
            : base(new Microsoft.Extensions.Caching.SqlServer.SqlServerCache(options)) { }
    }
}
