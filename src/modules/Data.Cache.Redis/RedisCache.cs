using Microsoft.Extensions.Options;

namespace Ws.Core.Extensions.Data.Cache.Redis;

public class RedisCache : DistributedCache
{
    public RedisCache(IOptions<Microsoft.Extensions.Caching.Redis.RedisCacheOptions> options)
        : base(new Microsoft.Extensions.Caching.Redis.RedisCache(options)) { }
}
public class RedisCache<T> : DistributedCache<T> where T : class
{
    public RedisCache(IOptions<Microsoft.Extensions.Caching.Redis.RedisCacheOptions> options)
        : base(new Microsoft.Extensions.Caching.Redis.RedisCache(options)) { }
}
