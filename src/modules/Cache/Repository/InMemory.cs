using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using core.Extensions.Data;
using Microsoft.Extensions.Caching.Memory;

namespace core.Extensions.Cache.Repository
{
    public class InMemory<T> : ICachedRepository<T> where T : IEntity
    {
        private string _key => $"CachedRepositoryOf{typeof(T).ToString()}";
        private static IMemoryCache _cache;
        private List<T> _collection;

        public InMemory() { }

        public InMemory(IMemoryCache cache,IRepository<T> repository)
        {
            if (_cache == null) _cache = cache;
            if (!_cache.TryGetValue(_key, out _collection))
            {
                _collection = repository.List.ToList();
                _cache.Set(_key, _collection);
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
        private void Save() {
            _cache.Set(_key, _collection);
        }

    }
}