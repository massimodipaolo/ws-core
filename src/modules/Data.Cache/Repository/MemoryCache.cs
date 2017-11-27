using core.Extensions.Data.Cache;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq;

namespace core.Extensions.Data.Repository
{
    public class MemoryCache<T> : CachedRepository<T> where T : IEntity
    {   
        private static IMemoryCache _cache;        

        public MemoryCache() { }

        public MemoryCache(IMemoryCache cache,IRepository<T> repository)
        {
            if (_cache == null) _cache = cache;
            if (!_cache.TryGetValue(_key, out _collection))
            {                
                _collection = repository.List.ToList();
                Save();         
            }
        }

        protected override void Save() {
            _cache.Set(_key, _collection);
        }

    }
}