using core.Extensions.Data.Cache;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace core.Extensions.Data.Repository
{
    public class DistributedCache<T> : CachedRepository<T> where T : IEntity
    {        
        private static IDistributedCache _cache;

        public DistributedCache() { }

        public DistributedCache(IDistributedCache cache, IRepository<T> repository)
        {
            if (_cache == null) _cache = cache;
            var result = _cache.Get(_key);
            if (result != null)
            {   
               _collection = JsonConvert.DeserializeObject<List<T>>(Encoding.UTF8.GetString(result));                
            }
            if (_collection == null)
            {
                _collection = repository.List.ToList();
                Save();
            }
        }

        protected override void Save()
        {       
            _cache.Set(_key, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(_collection)));
        }

    }
}