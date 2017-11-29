using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace core.Extensions.Data.Cache
{
    public class DistributedCache : ICache
    {
        private static IDistributedCache _cache;

        public DistributedCache() { }

        public DistributedCache(IDistributedCache cache)
        {
            if (_cache == null) _cache = cache;
        }        

        public object Get(string key)
        {            
            return Get<object>(key);
        }

        public T Get<T>(string key)
        {
            var result = _cache.Get(key);
            if (result != null)
            {
                return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(result));
            }
            return default(T);
        }

        public void Set(string key, object value)
        {            
            _cache.Set(key, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)));
        }
    }
}
