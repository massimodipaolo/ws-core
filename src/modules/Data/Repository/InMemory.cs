using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ws.Core.Extensions.Data.Repository
{
    public class InMemory<T, TKey> : BaseRepository, IRepository<T, TKey> where T : class, IEntity<TKey> where TKey : IEquatable<TKey>
    {
        private static List<T> _collection = new ();

        public InMemory() { }

        public InMemory(List<T> data)
        {
            _collection = data;
        }

        IQueryable<T> IRepository<T>.List => _collection.AsQueryable();

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
                _collection.Add(entity);
        }

        public void AddMany(IEnumerable<T> entities)
        {
            if (entities != null && entities.Any())
                _collection.AddRange(entities);
        }

        public void Update(T entity)
        {
            if (entity != null)
            {
                var item = Find(entity.Id);
                if (item != null)
                    _collection[_collection.IndexOf(item)] = entity;
            }
        }

        public void UpdateMany(IEnumerable<T> entities)
        {
            if (entities != null && entities.Any())
                _collection
               .Join(entities, o => o.Id, i => i.Id, (o, i) => (o, i))
               .AsParallel()
               .ForAll(_ => _collection[_collection.IndexOf(_.o)] = _.i);
        }

        public void Merge(IEnumerable<T> entities, RepositoryMergeOperation operation = RepositoryMergeOperation.Upsert)
        {
            if (entities != null && entities.Any())
                switch (operation)
                {
                    case RepositoryMergeOperation.Upsert:
                        _collection = entities.Union(_collection, new EntityComparer<T, TKey>()).ToList();
                        break;
                    case RepositoryMergeOperation.Sync:
                        _collection = entities.ToList();
                        break;
                }
        }

        public void Delete(T entity)
        {
            if (entity != null)
                _collection.RemoveAll(_ => _.Id.Equals(entity.Id));
        }

        public void DeleteMany(IEnumerable<T> entities)
        {
            if (entities != null && entities.Any())
                _collection.RemoveAll(_ => entities.Any(__ => __.Id.Equals(_.Id)));
        }
    }
}
