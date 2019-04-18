using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ws.Core.Extensions.Data.Repository
{
    public class InMemory<T, TKey> : IRepository<T, TKey> where T : class, IEntity<TKey> where TKey : IEquatable<TKey>
    {
        private static List<T> _collection = new List<T>();

        public InMemory() { }

        public InMemory(List<T> data)
        {
            _collection = data;
        }

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
            _collection.Add(entity);
        }

        public void Update(T entity)
        {
            _collection = _collection.Select(_ => _.Id.Equals(entity.Id) ? entity : _).ToList();
        }

        public void Merge(IEnumerable<T> entities, RepositoryMergeOperation operation = RepositoryMergeOperation.Upsert)
        {
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
            _collection.RemoveAll(_ => _.Id.Equals(entity.Id));
        }
        
    }
}
