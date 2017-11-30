using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace core.Extensions.Data.Cache
{    
    public class MemoryCache : ICache
    {
        private static IMemoryCache _cache;

        public MemoryCache() { }

        public MemoryCache(IMemoryCache cache)
        {
            if (_cache == null) _cache = cache;
        }

        public object Get(string key)
        {
            _cache.TryGetValue(key, out object _value);            
            return _value;
        }

        public T Get<T>(string key)
        {
            _cache.TryGetValue(key, out T _value);
            return _value;
        }

        public void Set(string key, object value)
        {            
            _cache.Set(key, value);
        }

        public void Set(string key, object value, ICacheEntryOptions options)
        {
            _cache.Set(key, value, new MemoryCacheEntryOptions() { AbsoluteExpiration = options.AbsoluteExpiration, AbsoluteExpirationRelativeToNow = options.AbsoluteExpirationRelativeToNow, SlidingExpiration = options.SlidingExpiration });
        }
    }
}