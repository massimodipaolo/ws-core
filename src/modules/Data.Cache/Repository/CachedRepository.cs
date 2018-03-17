using System;
using core.Extensions.Data.Cache;
using System.Collections.Generic;
using System.Linq;

namespace core.Extensions.Data.Repository
{
    public class CachedRepository<T, TKey> : ICacheRepository<T, TKey> where T : IEntity<TKey> where TKey : IEquatable<TKey>
    {
        private static ICache _cache;
        //private string _key => $"cache:repository:{typeof(T).ToString()}";
        private string _key => CachedRepository<T, TKey>.Key;
        private List<T> _collection;


        public CachedRepository() { }

        public CachedRepository(ICache cache, IRepository<T, TKey> repository)
        {
            if (_cache == null) _cache = cache;

            _collection = _cache.Get<List<T>>(_key);
            if (_collection == null)
            {
                _collection = repository.List.ToList();
                Save();
            }
        }

        public static string Key => $"cache:repository:{typeof(T).ToString()}";

        IQueryable<T> IRepository<T, TKey>.List => _collection.AsQueryable();

        public T Find(TKey Id)
        {
            return _collection.FirstOrDefault(_ => _.Id.Equals(Id));
        }

        public void Add(T entity)
        {
            _collection.Add(entity);
            Save();
        }

        public void Delete(T entity)
        {
            _collection.RemoveAll(_ => _.Id.Equals(entity.Id));
            Save();
        }

        public void Update(T entity)
        {
            _collection = _collection.Select(_ => _.Id.Equals(entity.Id) ? entity : _).ToList();
            Save();
        }
        private void Save()
        {

            _cache.Set(_key, _collection);
        }
    }
}
