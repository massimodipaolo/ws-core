using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace core.Data.Repository
{
    public class SqlServer<T> : IRepository<T> where T : Entity
    {
        private readonly AppDbContext _context;
        private DbSet<T> _collection;

        public SqlServer(AppDbContext context)
        {
            _context = context;
            _collection = _context.Set<T>();
        }

        public IQueryable<T> List => _collection.AsNoTracking().AsQueryable();

        public T Find(string Id)
        {
            return List.SingleOrDefault(_ => _.Id == Id);
        }

        public void Add(T entity)
        {
            if (entity != null)
            {
                _collection.Add(entity);
                _context.SaveChanges();
            }
        }

        public void Update(T entity)
        {
            if (entity != null)
            {
                _collection.Update(entity);
                _context.SaveChanges();
            }
        }

        public void Delete(T entity)
        {
            if (entity != null)
            {
                _collection.Remove(entity);
                _context.SaveChanges();
            }
        }

    }
}
