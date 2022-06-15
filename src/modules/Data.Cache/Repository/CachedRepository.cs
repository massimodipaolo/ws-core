using System;
using Ws.Core.Extensions.Data.Cache;
using System.Collections.Generic;
using System.Linq;

namespace Ws.Core.Extensions.Data.Repository
{
    public static class CachedRepository
    {
        internal static ICache? Cache { get; set; }
    }
    public class CachedRepository<T, TKey> : BaseRepository, ICacheRepository<T, TKey> where T : class, IEntity<TKey> where TKey : IEquatable<TKey>, IComparable<TKey>
    {
        private static string collectionKey => CachedRepository<T, TKey>.Key;
        private List<T>? collection { get; set; }

        public CachedRepository(ICache cache, IRepository<T, TKey> repository)
        {
            if (CachedRepository.Cache == null)
                CachedRepository.Cache = cache;

            collection = CachedRepository.Cache.Get<List<T>>(collectionKey);
            if (collection == null)
            {
                collection = repository.List.ToList();
                Save();
            }
        }

        public static string Key => $"cache:repository:{typeof(T)}";
        public static string EntityKey(TKey? Id) => $"{Key}:{Id?.ToString() ?? "-"}";

        IQueryable<T> IRepository<T>.List => (collection ?? Array.Empty<T>().ToList()).AsQueryable();

        public T? Find(TKey? Id)
        {
            return collection?.FirstOrDefault(_ => _.Id?.Equals(Id) == true) ?? default;
        }

        public IQueryable<T> Query(FormattableString command)
        {
            throw new NotImplementedException();
        }

        public void Add(T entity)
        {
            if (entity != null)
            {
                collection?.Add(entity);
                Save();
            }
        }

        public void AddMany(IEnumerable<T> entities)
        {
            if (entities != null && entities.Any())
            {
                collection?.AddRange(entities);
                Save();
            }
        }

        public void Update(T entity)
        {
            if (entity != null)
            {
                var item = Find(entity.Id);
                if (item != null && collection?.Any() == true)
                    collection[collection.IndexOf(item)] = entity;
                Save();
            }
        }

        public void UpdateMany(IEnumerable<T> entities)
        {
            if (entities != null && entities.Any())
            {
                collection?
                   .Join(entities, o => o.Id, i => i.Id, (o, i) => (o, i))
                   .AsParallel()
                   .ForAll(_ => collection[collection.IndexOf(_.o)] = _.i);
                Save();
            }
        }

        public void Merge(IEnumerable<T> entities, RepositoryMergeOperation operation = RepositoryMergeOperation.Upsert)
        {
            if (entities != null && entities.Any())
            {
                switch (operation)
                {
                    case RepositoryMergeOperation.Upsert:
                        collection = entities.Union(collection ?? Array.Empty<T>().ToList(), new EntityComparer<T, TKey>()).ToList();
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
                collection?.RemoveAll(_ => _.Id?.Equals(entity.Id) == true);
                Save();
            }
        }

        public void DeleteMany(IEnumerable<T> entities)
        {
            if (entities != null && entities.Any())
            {
                collection?.RemoveAll(_ => entities.Any(__ => __.Id?.Equals(_.Id) == true));
                Save();
            }
        }

        private void Save()
        {
            if (collection != null)
                CachedRepository.Cache?.Set(collectionKey, collection);
        }
    }
}
