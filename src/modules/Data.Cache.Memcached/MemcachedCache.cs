using Enyim.Caching.Configuration;
using Microsoft.Extensions.Logging;

namespace Ws.Core.Extensions.Data.Cache.Memcached;

public class MemcachedCache : DistributedCache
{    public MemcachedCache(ILoggerFactory logger, IMemcachedClientConfiguration config)
        : base(new Enyim.Caching.MemcachedClient(logger, config)) { }
}
public class MemcachedCache<T> : DistributedCache<T> where T : class
{
    public MemcachedCache(ILoggerFactory logger, IMemcachedClientConfiguration config)
        : base(new Enyim.Caching.MemcachedClient(logger,config)) { }
}