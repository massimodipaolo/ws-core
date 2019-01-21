using core.Extensions.Data;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace core.Extensions.Data.Repository
{
    public class EF<T, TKey> : IRepository<T, TKey> where T : class, IEntity<TKey> where TKey : IEquatable<TKey>
    {
        private readonly AppDbContext _context;
        private DbSet<T> _collection;

        public EF(AppDbContext context)
        {
            _context = context;
            _collection = _context.Set<T>();
        }

        public IQueryable<T> List => _collection.AsNoTracking().AsQueryable();

        public T Find(TKey Id)
        {
            return List.SingleOrDefault(_ => _.Id.Equals(Id));
        }

        public IQueryable<T> Query(FormattableString command)
        {
            return _context.Set<T>().FromSql(command);
        }

        public void Add(T entity)
        {
            if (entity != null)
            {
                _collection.Add(entity);
                _context.SaveChanges();
                //entity.OnChange(EntityChangeEventContext<TKey>.ActionTypes.Create);
            }
        }

        public void Update(T entity)
        {
            if (entity != null)
            {
                _collection.Update(entity);
                _context.SaveChanges();
                //entity.OnChange(EntityChangeEventContext<TKey>.ActionTypes.Update);
            }
        }

        public void Merge(IEnumerable<T> entities, RepositoryMergeOperation operation = RepositoryMergeOperation.Upsert)
        {
            if (entities != null)
            {
                var bulkConfig = new BulkConfig()
                {
                    UseTempDB = true,                    
                    UpdateByProperties = new[] { "Id" }.ToList()
                };

                using (var transaction = _context.Database.BeginTransaction())
                {
                    switch (operation)
                    {
                        case RepositoryMergeOperation.Upsert:
                            _context.BulkInsertOrUpdate<T>(entities.ToList(), bulkConfig);
                            break;
                        case RepositoryMergeOperation.Sync:
                            _context.BulkInsertOrUpdateOrDelete<T>(entities.ToList(), bulkConfig);
                            break;
                    }
                    transaction.Commit();
                }

            }
        }

        public void Delete(T entity)
        {
            if (entity != null)
            {
                _collection.Remove(entity);
                _context.SaveChanges();
                //entity.OnChange(EntityChangeEventContext<TKey>.ActionTypes.Delete);
            }
        }

    }
}

