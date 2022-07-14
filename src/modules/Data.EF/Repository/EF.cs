using Ws.Core.Extensions.Data.EF;
using Ws.Core.Extensions.Data.Repository.EF;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;

namespace Ws.Core.Extensions.Data.Repository
{
    public class EFBase : BaseRepository
    {
        protected static Extensions.Data.EF.Options.IncludeNavigationPropertiesConfig _includeOptions { get; set; } = new Extensions.Data.EF.Extension().Options?.IncludeNavigationProperties ?? new Extensions.Data.EF.Options.IncludeNavigationPropertiesConfig();
        protected IServiceProvider? _provider { get; set; }
    }
    public class EF<T, TKey> : EFBase, IRepository<T, TKey>
        where T : class, IEntity<TKey>
        where TKey : IEquatable<TKey>, IComparable<TKey>
    {

        protected readonly Extensions.Data.EF.DbContext _context;
        protected DbSet<T> _collection;


        public EF(Extensions.Data.EF.DbContext context, Data.DbConnection dbConnection, IServiceProvider provider)
            : this(context, provider)
        {
            _context.Database.GetDbConnection().ConnectionString = dbConnection.ConnectionString;
        }

        public EF(Extensions.Data.EF.DbContext context, IServiceProvider provider)
        {
            _context = context;
            _collection = _context.Set<T>();
            _provider = provider;
        }

        private IQueryable<T> _list => _collection.AsNoTracking().AsQueryable();

        private IQueryable<T> _query(Extensions.Data.EF.Options.IncludeNavigationPropertiesConfig.Operation? op)
        {
            var query = _list;
            var customRule = op?.Explicit?.FirstOrDefault(_ => _.Entity == typeof(T).FullName);
            // custom navigation properties
            if (customRule != null && customRule.Paths != null && customRule.Paths.Any(_ => _.Any()))
            {
                foreach (var paths in customRule.Paths.Where(_ => _.Any()))
                    query = query.Include(string.Join('.', paths));
            }
            // load main navigation properties
            else if ((op?.Enable ?? false) ^ (op?.Except ?? Array.Empty<string>()).Contains(typeof(T).FullName))
            {
                Microsoft.EntityFrameworkCore.Metadata.IEntityType? _entityType = _context.Model.FindEntityType(typeof(T));
                if (_entityType != null)
                    foreach (var property in _entityType.GetNavigations())
                        query = query.Include(property.Name);
            }
            return query;
        }

        public virtual IIncludableJoin<T, TProperty> IncludeJoin<TProperty>(Expression<Func<T, TProperty>> navigationProperty)
        => _list.IncludeJoin(navigationProperty);

        public virtual IQueryable<T> List => _query(_includeOptions.List);

        public virtual T? Find(TKey? Id)
        {
            return _query(_includeOptions.Find)?.SingleOrDefault(_ => _.Id != null && _.Id.Equals(Id));
        }

        public IQueryable<T> Query(FormattableString command)
        {
            return _context.Set<T>().FromSqlInterpolated(command);
        }

        public virtual void Add(T entity)
        {
            if (entity != null)
            {
                _collection.Add(entity);
                _context.SaveChanges();
            }
        }

        public virtual void AddMany(IEnumerable<T> entities)
        {
            if (entities != null && entities.Any())
            {
                _collection.AddRange(entities);
                _context.SaveChanges();
            }
        }

        public virtual void Update(T entity)
        {
            if (entity != null)
            {
                _collection.Update(entity);
                _context.SaveChanges();
            }
        }

        public virtual void UpdateMany(IEnumerable<T> entities)
        {
            if (entities != null && entities.Any())
            {
                _collection.UpdateRange(entities);
                _context.SaveChanges();
            }
        }

        public virtual void Merge(IEnumerable<T> entities, RepositoryMergeOperation operation = RepositoryMergeOperation.Upsert)
        {
            if (entities != null && entities.Any())
            {
                var keys = entities.Select(_ => _.Id);
                var joined = _collection
                    .AsNoTracking()
                    .Where(_ => keys.Contains(_.Id))
                    .ToList().AsEnumerable()
                    .Join(entities, c => c.Id, e => e.Id, (c, e) => new { c, e })
                    .ToList();

                if (operation == RepositoryMergeOperation.Sync)
                    _mergeDelete(keys);

                var toUpdate = joined
                      .Where(_ => !_.c.Equals(_.e)) // First fast check
                      .Where(_ => Newtonsoft.Json.JsonConvert.SerializeObject(_.c) != Newtonsoft.Json.JsonConvert.SerializeObject(_.e)) // Deeper comparison
                      .Select(_ => _.e)
                      .ToList();
                _mergeUpdate(toUpdate);

                var toAdd = (joined != null && joined.Any() ? entities.Except(joined.Select(_ => _.e), new EntityComparer<T, TKey>()) : entities).ToList();
                _mergeAdd(toAdd);

                _context.SaveChanges();
            }
        }

        private void _mergeDelete(IEnumerable<TKey?> keys)
        {
            var toDelete = _collection.Where(_ => !keys.Contains(_.Id));
            if (toDelete != null && toDelete.Any())
                _collection.RemoveRange(toDelete);
        }
        private void _mergeUpdate(IEnumerable<T> toUpdate)
        {
            if (toUpdate != null && toUpdate.Any())
                _collection.UpdateRange(toUpdate);
        }
        private void _mergeAdd(IEnumerable<T> toAdd)
        {
            if (toAdd != null && toAdd.Any())
                _collection.AddRange(toAdd);
        }

        public virtual void Delete(T entity)
        {
            if (entity != null)
            {
                _collection.Remove(entity);
                _context.SaveChanges();
            }
        }

        public virtual void DeleteMany(IEnumerable<T> entities)
        {
            if (entities != null && entities.Any())
            {
                _collection.RemoveRange(entities);
                _context.SaveChanges();
            }
        }

        protected Microsoft.EntityFrameworkCore.Infrastructure.DatabaseFacade? db<TContext>() where TContext : Extensions.Data.EF.DbContext
        {
            Microsoft.EntityFrameworkCore.Infrastructure.DatabaseFacade? df;
            try
            {
                df = _context.Database;
            }
            catch  // context disposed
            {
                df = _provider?.GetService<TContext>()?.Database;
            }
            return df;
        }

    }

}

