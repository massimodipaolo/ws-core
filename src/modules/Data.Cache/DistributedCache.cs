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
            _client.Set(key, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)));
        }

        public void Set(string key, object value, ICacheEntryOptions options)
        {
            _client.Set(key, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)), new DistributedCacheEntryOptions() { AbsoluteExpiration = options.AbsoluteExpiration, AbsoluteExpirationRelativeToNow = options.AbsoluteExpirationRelativeToNow, SlidingExpiration = options.SlidingExpiration });
        }

        public void Remove(string key)
        {
            _client.Remove(key);
        }
    }
}
