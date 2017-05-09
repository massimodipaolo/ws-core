using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace core.Data
{
    public interface IRepository<T> where T : IEntity
    {
        IQueryable<T> List { get; }
        T Find(string Id);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
