using core.Extensions.Data.Cache;
using System.Collections.Generic;
using System.Linq;

namespace core.Extensions.Data.Repository
{
    public class CachedRepository<T> : ICachedRepository<T> where T : IEntity
    {
        protected string _key => $"CachedRepositoryOf{typeof(T).ToString()}";
        protected List<T> _collection;

        IQueryable<T> IRepository<T>.List => _collection.AsQueryable();

        public T Find(string Id)
        {
            return _collection.Where(_ => _.Id == Id).FirstOrDefault();
        }

        public void Add(T entity)
        {
            _collection.Add(entity);
            Save();
        }

        public void Delete(T entity)
        {
            _collection.RemoveAll(_ => _.Id == entity.Id);
            Save();
        }

        public void Update(T entity)
        {
            _collection = _collection.Select(_ => _.Id == entity.Id ? entity : _).ToList();
            Save();
        }
        protected virtual void Save()
        {
            
        }
    }
}
