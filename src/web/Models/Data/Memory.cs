using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace web.Data
{   
    public class Memory<T> : IRepository<T> where T : IEntity
    {
        ICollection<T> _collection;

        public Memory(ICollection<T> data)
        {
            _collection = data;
        }

        public IEnumerable<T> List
        {
            get
            {
                return _collection;
            }
        }

        public void Add(T entity)
        {
            _collection.Add(entity);
        }

        public void Delete(T entity)
        {            
            _collection.Remove(entity);
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
