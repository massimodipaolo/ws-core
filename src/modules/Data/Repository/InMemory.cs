using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace core.Extensions.Data.Repository
{
    public class InMemory<T, TKey> : IRepository<T, TKey> where T : IEntity<TKey> where TKey : IEquatable<TKey>
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

        public void Add(T entity)
        {
            _collection.Add(entity);
        }

        public void Delete(T entity)
        {
            _collection.RemoveAll(_ => _.Id.Equals(entity.Id));
        }

        public void Update(T entity)
        {
            _collection = _collection.Select(_ => _.Id.Equals(entity.Id) ? entity : _).ToList();
        }

    }
}
