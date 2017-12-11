using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace core.Extensions.Data.Cache
{
    public class MemoryCache : ICache
    {
        readonly IMemoryCache _client;

        public MemoryCache() { }

        public MemoryCache(IMemoryCache client)
        {
            _client = client;
        }

        public object Get(string key)
        {
            _client.TryGetValue(key, out object _value);
            return _value;
        }

        public T Get<T>(string key)
        {
            _client.TryGetValue(key, out T _value);
            return _value;
        }

        public void Set(string key, object value)
        {
            _client.Set(key, value);
        }

        public void Set(string key, object value, ICacheEntryOptions options)
        {
            _client.Set(key, value, new MemoryCacheEntryOptions() { AbsoluteExpiration = options.AbsoluteExpiration, AbsoluteExpirationRelativeToNow = options.AbsoluteExpirationRelativeToNow, SlidingExpiration = options.SlidingExpiration });
        }

        public void Remove(string key)
        {
            _client.Remove(key);
        }
    }
}