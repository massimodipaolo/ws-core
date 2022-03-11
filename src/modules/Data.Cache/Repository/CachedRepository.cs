using System;
using Ws.Core.Extensions.Data.Cache;
using System.Collections.Generic;
using System.Linq;

namespace Ws.Core.Extensions.Data.Repository
{
    public class CachedRepository<T, TKey> : BaseRepository, ICacheRepository<T, TKey> where T : class, IEntity<TKey> where TKey : IEquatable<TKey>
    {
        internal static ICache Cache { get; private set; }
        private static string collectionKey => CachedRepository<T,TKey>.Key;
        private List<T> collection;

        public CachedRepository() { }

        public CachedRepository(ICache cache, IRepository<T, TKey> repository)
        {
            if (CachedRepository<T, TKey>.Cache == null)
                CachedRepository<T, TKey>.Cache = cache;

            collection = CachedRepository<T, TKey>.Cache.Get<List<T>>(collectionKey);
            if (collection == null)
            {
                collection = repository.List.ToList();
                Save();
            }
        }

        public static string Key => $"cache:repository:{typeof(T)}";
        public static string EntityKey(TKey Id) => $"{Key}:{Id}";

        IQueryable<T> IRepository<T>.List => collection.AsQueryable();

        public T Find(TKey Id)
        {
            return collection.FirstOrDefault(_ => _.Id.Equals(Id));
        }

        public IQueryable<T> Query(FormattableString command)
        {
            throw new NotImplementedException();
        }

        public void Add(T entity)
        {
            if (entity != null)
            {
                collection.Add(entity);
                Save();
            }
        }

        public void AddMany(IEnumerable<T> entities)
        {
            if (entities != null && entities.Any())
            {
                collection.AddRange(entities);
                Save();
            }
        }

        public void Update(T entity)
        {
            if (entity != null)
            {
                var item = Find(entity.Id);
                if (item != null)
                    collection[collection.IndexOf(item)] = entity;
                Save();
            }
        }

        public void UpdateMany(IEnumerable<T> entities)
        {
            if (entities != null && entities.Any())
            {
                collection
                   .Join(entities, o => o.Id, i => i.Id, (o, i) => (o, i))
                   .AsParallel()
                   .ForAll(_ => collection[collection.IndexOf(_.o)] = _.i);
                Save();
            }
        }

        public void Merge(IEnumerable<T> entities, RepositoryMergeOperation operation = RepositoryMergeOperation.Upsert)
        {
            if (entities != null && entities.Any()) {
                switch (operation)
                {
                    case RepositoryMergeOperation.Upsert:
                        collection = entities.Union(collection, new EntityComparer<T, TKey>()).ToList();
                        break;
                    case RepositoryMergeOperation.Sync:
                        collection = entities.ToList();
                        break;
                }
                Save();
            }
        }

        public void Delete(T entity)
        {
            if (entity != null)
            {
                collection.RemoveAll(_ => _.Id.Equals(entity.Id));
                Save();
            }
        }

        public void DeleteMany(IEnumerable<T> entities)
        {
            if (entities != null && entities.Any())
            {
                collection.RemoveAll(_ => entities.Any(__ => __.Id.Equals(_.Id)));
                Save();
            }
        }

        private void Save()
        {
            Cache.Set(collectionKey, collection);
        }
    }

    public class EntityChangeHandler<T, TKey> : IEntityChangeEvent<T, TKey> where T : class, IEntity<TKey> where TKey : IEquatable<TKey>
    {
        public int Priority => 0;

        public void HandleEvent(EntityChangeEventContext ctx)
        {

            var entity = (T)ctx.Entity;

            ICache _cache = CachedRepository<T, TKey>.Cache;

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
            CachedRepository<T, TKey>.Cache.Clear();
        }
    }
}
