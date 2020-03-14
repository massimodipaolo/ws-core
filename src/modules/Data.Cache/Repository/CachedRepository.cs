using System;
using Ws.Core.Extensions.Data.Cache;
using System.Collections.Generic;
using System.Linq;

namespace Ws.Core.Extensions.Data.Repository
{
    public class CachedRepository<T, TKey> : ICacheRepository<T, TKey> where T : class, IEntity<TKey> where TKey : IEquatable<TKey>
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

        public IQueryable<T> Query(FormattableString command)
        {
            throw new NotImplementedException();
        }

        public void Add(T entity)
        {
            if (entity != null)
            {
                _collection.Add(entity);
                Save();
            }
        }

        public void AddMany(IEnumerable<T> entities)
        {
            if (entities != null && entities.Any())
            {
                _collection.AddRange(entities);
                Save();
            }
        }

        public void Update(T entity)
        {
            if (entity != null)
            {
                var item = Find(entity.Id);
                if (item != null)
                    _collection[_collection.IndexOf(item)] = entity;
                Save();
            }
        }

        public void UpdateMany(IEnumerable<T> entities)
        {
            if (entities != null && entities.Any())
            {
                _collection
                   .Join(entities, o => o.Id, i => i.Id, (o, i) => (o, i))
                   .AsParallel()
                   .ForAll(_ => _collection[_collection.IndexOf(_.o)] = _.i);
                Save();
            }
        }

        public void Merge(IEnumerable<T> entities, RepositoryMergeOperation operation = RepositoryMergeOperation.Upsert)
        {
            if (entities != null && entities.Any()) {
                switch (operation)
                {
                    case RepositoryMergeOperation.Upsert:
                        _collection = entities.Union(_collection, new EntityComparer<T, TKey>()).ToList();
                        break;
                    case RepositoryMergeOperation.Sync:
                        _collection = entities.ToList();
                        break;
                }
                Save();
            }
        }

        public void Delete(T entity)
        {
            if (entity != null)
            {
                _collection.RemoveAll(_ => _.Id.Equals(entity.Id));
                Save();
            }
        }

        public void DeleteMany(IEnumerable<T> entities)
        {
            if (entities != null && entities.Any())
            {
                _collection.RemoveAll(_ => entities.Any(__ => __.Id.Equals(_.Id)));
                Save();
            }
        }

        private void Save()
        {
            _cache.Set(_collectionKey, _collection);
        }
    }

    public class EntityChangeHandler<T, TKey> : IEntityChangeEvent<T, TKey> where T : class, IEntity<TKey> where TKey : IEquatable<TKey>
    {
        public int Priority => 0;

        public void HandleEvent(EntityChangeEventContext ctx)
        {

            var entity = (T)ctx.Entity;

            ICache _cache = CachedRepository<T, TKey>._cache;

            // sync entity
            string _key = CachedRepository<T, TKey>.EntityKey(entity.Id);
            switch (ctx.Action)
            {
                case EntityChangeEventContext.ActionTypes.Create:
                case EntityChangeEventContext.ActionTypes.Update:
                    _cache.Set(_key, entity);
                    break;
                case EntityChangeEventContext.ActionTypes.Delete:
                    _cache.Remove(_key);
                    break;
            }

            // sync entity collection
            CachedRepository<T, TKey>._cache.Clear();
        }
    }

    public class EntityChangeHandler<TKey> : IEntityChangeEvent<TKey> where TKey : IEquatable<TKey>
    {
        public int Priority => 0;

        public void HandleEvent(EntityChangeEventContext ctx)
        {
            var entity = (IEntity<TKey>)ctx.Entity;
            TKey id = entity.Id;
        }

    }
    public class EntityChangeHandler : IEntityChangeEvent
    {
        public int Priority => 0;

        public void HandleEvent(EntityChangeEventContext ctx)
        {
            var entity = ctx.Entity;
        }
    }


}
