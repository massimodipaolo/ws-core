using System;
using core.Extensions.Data.Cache;
using System.Collections.Generic;
using System.Linq;

namespace core.Extensions.Data.Repository
{
    public class CachedRepository<T> : ICacheRepository<T> where T : IEntity
    {
        private static ICache _cache;
        private string _key => $"CachedRepositoryOf{typeof(T).ToString()}";
        private List<T> _collection;
        

        public CachedRepository() { }

        public CachedRepository(ICache cache, IRepository<T> repository)
        {
            if (_cache == null) _cache = cache;

            _collection = _cache.Get<List<T>>(_key);
            if (_collection == null){
                _collection = repository.List.ToList();
                Save();
            }
        }

        IQueryable<T> IRepository<T>.List => _collection.AsQueryable();

        public T Find(string Id)
        {
            return _collection.Where(_ => _.Id == Id).FirstOrDefault();
        }

        public void Add(T entity)
        {
            _collection.Add(entity);
            Save();
        }

        public void Delete(T entity)
        {
            _collection.RemoveAll(_ => _.Id == entity.Id);
            Save();
        }

        public void Update(T entity)
        {
            _collection = _collection.Select(_ => _.Id == entity.Id ? entity : _).ToList();
            Save();
        }
        private void Save()
        {
            _cache.Set(_key, _collection);
        }
    }
}
