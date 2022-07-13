using Microsoft.Extensions.Options;

namespace Ws.Core.Extensions.Data.Cache.Redis;

public class RedisCache : DistributedCache
{
    public RedisCache(IOptions<Microsoft.Extensions.Caching.Redis.RedisCacheOptions> options, IExpirationTier<RedisCache> expirationTier)
        : base(new Microsoft.Extensions.Caching.Redis.RedisCache(options), expirationTier) { }
}
public class RedisCache<T> : RedisCache, ICache<T> where T : class
{
    public RedisCache(IOptions<Microsoft.Extensions.Caching.Redis.RedisCacheOptions> options, IExpirationTier<RedisCache> expirationTier)
        : base(options, expirationTier) { }
}
