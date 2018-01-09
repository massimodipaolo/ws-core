using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace core.Extensions.Data.Cache
{
    public class DistributedCache : ICache
    {
        readonly IDistributedCache _client;
        private static string _keyCollection = "___all_keys";

        public DistributedCache() { }

        public DistributedCache(IDistributedCache client)
        {
            _client = client;
        }

        public object Get(string key)
        {
            return Get<object>(key);
        }

        public T Get<T>(string key)
        {
            var result = _client.Get(key);
            if (result != null)
            {
                return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(result));
            }
            return default(T);
        }

        public void Set(string key, object value)
        {
            Set(key, value, null);
        }

        public void Set(string key, object value, ICacheEntryOptions options)
        {
            var _options = new DistributedCacheEntryOptions();
            if (options != null)
            {
                _options.AbsoluteExpiration = options.AbsoluteExpiration;
                _options.AbsoluteExpirationRelativeToNow = options.AbsoluteExpirationRelativeToNow;
                _options.SlidingExpiration = options.SlidingExpiration;
            }
            _client.Set(key, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)), _options);

            SyncAllKeys(key);

        }

        private void SyncAllKeys(string key)
        {
            if (key != _keyCollection)
            {
                var _all_keys = Get<List<string>>(_keyCollection) ?? new List<string>();                
                if (!_all_keys.Contains(key))
                {
                    _all_keys.Add(key);
                    Set(_keyCollection, _all_keys, new CacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30) });
                }
            }
        }

        public void Remove(string key)
        {
            _client.Remove(key);
        }

        public void Clear()
        {
            foreach (var k in Get<List<string>>(_keyCollection))
                Remove(k);

            Set(_keyCollection, new List<string>(), new CacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30) });
        }
    }
}
