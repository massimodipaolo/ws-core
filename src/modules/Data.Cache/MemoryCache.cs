using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Linq;
using System.Collections.Concurrent;

namespace Ws.Core.Extensions.Data.Cache
{
    public class MemoryCache<T>: MemoryCache, ICache<T> where T: class {
        public MemoryCache(IMemoryCache client) : base(client) {}
    }
    public class MemoryCache : ICache
    {
        protected readonly IMemoryCache? _client;
        private static CancellationTokenSource _resetCacheToken { get; set; } = new ();
        private static IProducerConsumerCollection<string> _keys { get; set; } = new ConcurrentBag<string>();
        public MemoryCache() { }

        public MemoryCache(IMemoryCache client)
        {
            _client = client;
        }

        public IEnumerable<string> Keys => _keys;

        public object? Get(string key) => Get<object>(key);

        public T? Get<T>(string key)
        {
            if (_client != null)
            {
                _client.TryGetValue(key, out T _value);
                return _value;
            }
            return default;
        }

        public void Set(string key, object value)
        {
            _client?.Set(key, value, new CancellationChangeToken(_resetCacheToken.Token));
            _keys.TryAdd(key);
        }

        public void Set(string key, object value, ICacheEntryOptions options)
        {
            var _options = new MemoryCacheEntryOptions() { AbsoluteExpiration = options.AbsoluteExpiration, AbsoluteExpirationRelativeToNow = options.AbsoluteExpirationRelativeToNow, SlidingExpiration = options.SlidingExpiration };
            _options.AddExpirationToken(new CancellationChangeToken(_resetCacheToken.Token));
            _client?.Set(key, value, _options);
            _keys.TryAdd(key);
        }

        public void Remove(string key)
        {
            _client?.Remove(key);
            _ = _keys.TryTake(out _);
        }

        public void Clear()
        {
            if (_resetCacheToken != null && !_resetCacheToken.IsCancellationRequested && _resetCacheToken.Token.CanBeCanceled)
            {
                _resetCacheToken.Cancel();
                _resetCacheToken.Dispose();
            }
            _resetCacheToken = new CancellationTokenSource();
            _keys = new ConcurrentBag<string>();
        }
    }
}