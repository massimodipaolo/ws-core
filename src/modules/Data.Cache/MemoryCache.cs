using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace core.Extensions.Data.Cache
{
    public class MemoryCache : ICache
    {
        readonly IMemoryCache _client;
        private static CancellationTokenSource _resetCacheToken = new CancellationTokenSource();

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
            _client.Set(key, value, new CancellationChangeToken(_resetCacheToken.Token));
        }

        public void Set(string key, object value, ICacheEntryOptions options)
        {
            var _options = new MemoryCacheEntryOptions() { AbsoluteExpiration = options.AbsoluteExpiration, AbsoluteExpirationRelativeToNow = options.AbsoluteExpirationRelativeToNow, SlidingExpiration = options.SlidingExpiration };
            _options.AddExpirationToken(new CancellationChangeToken(_resetCacheToken.Token));            
            _client.Set(key, value, _options);
        }

        public void Remove(string key)
        {
            _client.Remove(key);                 
        }

        public void Clear()
        {
            if (_resetCacheToken != null && !_resetCacheToken.IsCancellationRequested && _resetCacheToken.Token.CanBeCanceled)
            {
                _resetCacheToken.Cancel();
                _resetCacheToken.Dispose();
            }
            _resetCacheToken = new CancellationTokenSource();
        }
    }
}