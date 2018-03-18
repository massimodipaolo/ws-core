using System;
using core.Extensions.Data.Cache;
using System.Collections.Generic;
using System.Linq;

namespace core.Extensions.Data.Repository
{
    public class CachedRepository<T, TKey> : ICacheRepository<T, TKey> where T : IEntity<TKey> where TKey : IEquatable<TKey>
    {
        internal static ICache _cache { get; set; }
        private string _collectionKey => CachedRepository<T, TKey>.Key;
        private List<T> _collection;


        public CachedRepository() { }

        public CachedRepository(ICache cache, IRepository<T, TKey> repository)
        {
            if (_cache == null)
                _cache = cache;

            _collection = _cache.Get<List<T>>(_collectionKey);
            if (_collection == null)
            {
                _collection = repository.List.ToList();
                Save();
            }
        }

        public static string Key => $"cache:repository:{typeof(T).ToString()}";
        public static string EntityKey(TKey Id) => $"{Key}:{Id}";

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
            _cache.Set(_collectionKey, _collection);
        }
    }

    public class EntityChangeHandler<T, TKey> : IEntityChangeEvent<TKey> where T : IEntity<TKey> where TKey : IEquatable<TKey>
    {
        public int Priority => 0;

        public virtual void HandleEvent(EntityChangeEventContext<TKey> ctx)
        {
            ICache _cache = CachedRepository<T, TKey>._cache;

            // sync entity
            string _key = CachedRepository<T, TKey>.EntityKey(ctx.Entity.Id);
            switch (ctx.Action)
            {
                case EntityChangeEventContext<TKey>.ActionTypes.Create:
                case EntityChangeEventContext<TKey>.ActionTypes.Update:
                    _cache.Set(_key, ctx.Entity);
                    break;
                case EntityChangeEventContext<TKey>.ActionTypes.Delete:
                    _cache.Remove(_key);
                    break;
            }

            // sync entity collection
            CachedRepository<T, TKey>._cache.Clear();
        }
    }

}
