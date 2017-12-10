using System;
using Enyim.Caching;

namespace core.Extensions.Data.Cache.Memcached
{
    public class MemcachedCache : core.Extensions.Data.Cache.ICache
    {
        readonly IMemcachedClient _client;

        public MemcachedCache(IMemcachedClient client)
        {
            _client = client;
        }

        public object Get(string key)
        {
            return _client.Get(key);
        }

        public T Get<T>(string key)
        {
            return _client.Get<T>(key);
        }

        public void Set(string key, object value)
        {
            _client.Store(Enyim.Caching.Memcached.StoreMode.Set, key, value);
        }

        public void Set(string key, object value, ICacheEntryOptions options)
        {
            DateTime ExpireAt = DateTime.Now;
            if (options.AbsoluteExpiration != null)
                ExpireAt = options.AbsoluteExpiration.Value.DateTime;
            else if (options.AbsoluteExpirationRelativeToNow != null)
                ExpireAt = DateTime.Now.AddTicks(options.AbsoluteExpirationRelativeToNow.Value.Ticks);
            else if (options.SlidingExpiration != null)
                ExpireAt = DateTime.Now.AddTicks(options.SlidingExpiration.Value.Ticks);
            else
                ExpireAt = DateTime.Now.AddTicks(CacheEntryOptions.Expiration.Never.AbsoluteExpirationRelativeToNow.Value.Ticks);

            _client.Store(Enyim.Caching.Memcached.StoreMode.Set, key, value, ExpireAt);
        }
    }
}
