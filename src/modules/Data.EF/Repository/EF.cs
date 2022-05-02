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
    public class EF<TContext,T, TKey> : BaseRepository, IRepository<T, TKey> where TContext: Extensions.Data.EF.DbContext where T : class, IEntity<TKey> where TKey : IEquatable<TKey>
    {
        private static Extensions.Data.EF.Options.IncludeNavigationPropertiesConfig _includeOptions { get; set; } = new Extensions.Data.EF.Extension().Options?.IncludeNavigationProperties ?? new Extensions.Data.EF.Options.IncludeNavigationPropertiesConfig();
        protected readonly TContext _context;
        protected DbSet<T> _collection;
        protected IServiceProvider _provider { get; set; }
        public EF(TContext context, IServiceProvider provider)
        {
            _context = context;
            _collection = _context.Set<T>();
            _provider = provider;
        }

        private IQueryable<T> _list => _collection.AsNoTracking().AsQueryable();

        private IQueryable<T> _query(Extensions.Data.EF.Options.IncludeNavigationPropertiesConfig.Operation op)
        {
            var query = _list;
            var customRule = op?.Explicit?.FirstOrDefault(_ => _.Entity == typeof(T).FullName);
            // custom navigation properties
            if (customRule != null && customRule.Paths != null && customRule.Paths.Any(_ => _.Any()))
            {
                foreach (var paths in customRule.Paths.Where(_ => _.Any()))
                    query = query.Include(string.Join('.', paths));
            }
            // load main navigation propertire
            else if ((op?.Enabled ?? false) ^ (op?.Except ?? new List<string>()).Contains(typeof(T).FullName))
            {
                foreach (var property in _context.Model.FindEntityType(typeof(T)).GetNavigations())
                    query = query.Include(property.Name);
            }
            return query;
        }

        public virtual IIncludableJoin<T, TProperty> IncludeJoin<TProperty>(Expression<Func<T, TProperty>> navigationProperty)
        {
            return (_list.IncludeJoin(navigationProperty));
        }

        public virtual IQueryable<T> List => _query(_includeOptions.List);

        public virtual T Find(TKey Id)
        {
            return _query(_includeOptions.Find).SingleOrDefault(_ => _.Id.Equals(Id));
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
                //entity.OnChange(EntityChangeEventContext<TKey>.ActionTypes.Create);
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
                //entity.OnChange(EntityChangeEventContext<TKey>.ActionTypes.Update);
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
                var joined = _collection
                    .Join(entities, c => c.Id, e => e.Id, (c, e) => new { c, e })
                    .ToList();

                if (operation == RepositoryMergeOperation.Sync)
                {
                    var toDelete = _collection.Where(_ => !entities.Any(__ => __.Id.Equals(_.Id)));
                    if (toDelete != null && toDelete.Any())
                        _collection.RemoveRange(toDelete);
                }

                var toUpdate = joined
                    .Where(_ => !_.c.Equals(_.e)) // First fast check
                    .Where(_ => Newtonsoft.Json.JsonConvert.SerializeObject(_.c) != Newtonsoft.Json.JsonConvert.SerializeObject(_.e)) // Deeper comparison
                    .Select(_ => _.e);
                if (toUpdate != null && toUpdate.Any())
                    _collection.UpdateRange(toUpdate);

                var toAdd = joined != null && joined.Any() ? entities.Except(joined.Select(_ => _.e), new EntityComparer<T, TKey>()) : entities;
                if (toAdd != null && toAdd.Any())
                    _collection.AddRange(toAdd);

                _context.SaveChanges();
            }
        }

        public virtual void Delete(T entity)
        {
            if (entity != null)
            {
                _collection.Remove(entity);
                _context.SaveChanges();
                //entity.OnChange(EntityChangeEventContext<TKey>.ActionTypes.Delete);
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

        protected Microsoft.EntityFrameworkCore.Infrastructure.DatabaseFacade db()
        {
            Microsoft.EntityFrameworkCore.Infrastructure.DatabaseFacade df;
            try
            {
                df = _context.Database;
            }
            catch  // context disposed
            {
                df = _provider.GetService<TContext>()?.Database;
            }
            return df;
        }

    }

}

