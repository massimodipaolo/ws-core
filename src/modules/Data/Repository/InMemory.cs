using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace core.Data.Repository
{
    public class InMemory<T> : IRepository<T> where T : IEntity
    {
        private static List<T> _collection = new List<T>();

        public InMemory() { }

        public InMemory(List<T> data)
        {
            _collection = data;
        }

        IQueryable<T> IRepository<T>.List => _collection.AsQueryable();

        public void Add(T entity)
        {
            _collection.Add(entity);
        }

        public void Delete(T entity)
        {
            _collection.RemoveAll(_ => _.Id == entity.Id);
        }

        public void Update(T entity)
        {
            _collection = _collection.Select(_ => _.Id == entity.Id ? entity : _).ToList();
        }

        public T Find(string Id)
        {
            return _collection.Where(_ => _.Id == Id).FirstOrDefault();
        }
    }
}
