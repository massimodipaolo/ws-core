using System;
using core.Extensions.Data.Cache;
using System.Collections.Generic;
using System.Linq;

namespace core.Extensions.Data.Repository
{
    public class CachedRepository<T> : ICachedRepository<T> where T : IEntity
    {
        private static ICache _cache;
        private string _key => $"CachedRepositoryOf{typeof(T).ToString()}";
        private Lazy<List<T>> _collection;
        

        public CachedRepository() { }

        public CachedRepository(ICache cache, IRepository<T> repository)
        {
            if (_cache == null) _cache = cache;

            _collection = new Lazy<List<T>>(() => {
                var value = _cache.Get<List<T>>(_key);
                if (value == null)
                {
                    value = repository.List.ToList();
                    Save();
                }
                return value;
            });
        }

        IQueryable<T> IRepository<T>.List => _collection.Value.AsQueryable();

        public T Find(string Id)
        {
            return _collection.Value.Where(_ => _.Id == Id).FirstOrDefault();
        }

        public void Add(T entity)
        {
            _collection.Value.Add(entity);
            Save();
        }

        public void Delete(T entity)
        {
            _collection.Value.RemoveAll(_ => _.Id == entity.Id);
            Save();
        }

        public void Update(T entity)
        {
            _collection = new Lazy<List<T>>(() => _collection.Value.Select(_ => _.Id == entity.Id ? entity : _).ToList());
            Save();
        }
        private void Save()
        {
            _cache.Set(_key, _collection.Value);
        }
    }
}
