using Enyim.Caching.Configuration;
using Microsoft.Extensions.Logging;

namespace Ws.Core.Extensions.Data.Cache.Memcached;

public class MemcachedCache : DistributedCache
{
    public MemcachedCache(ILoggerFactory logger, IMemcachedClientConfiguration config, IExpirationTier<MemcachedCache> expirationTier)
        : base(new Enyim.Caching.MemcachedClient(logger, config), expirationTier) { }
}
public class MemcachedCache<T> : MemcachedCache, ICache<T> where T : class
{
    public MemcachedCache(ILoggerFactory logger, IMemcachedClientConfiguration config, IExpirationTier<MemcachedCache> expirationTier)
        : base(logger, config, expirationTier) { }
}