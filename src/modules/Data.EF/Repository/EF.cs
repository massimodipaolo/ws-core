using core.Extensions.Data.EF;
using core.Extensions.Data.Repository.EF;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace core.Extensions.Data.Repository
{
    public class EF<T, TKey> : IRepository<T, TKey> where T : class, IEntity<TKey> where TKey : IEquatable<TKey>
    {
        private static core.Extensions.Data.EF.Options.IncludeNavigationPropertiesConfig _includeOptions { get; set; } = new core.Extensions.Data.EF.Extension()._options?.IncludeNavigationProperties ?? new core.Extensions.Data.EF.Options.IncludeNavigationPropertiesConfig();
        protected readonly AppDbContext _context;
        protected DbSet<T> _collection;

        public EF(AppDbContext context)
        {
            _context = context;
            _collection = _context.Set<T>();
        }

        private IQueryable<T> _list => _collection.AsNoTracking().AsQueryable();

        private IQueryable<T> _query(core.Extensions.Data.EF.Options.IncludeNavigationPropertiesConfig.Operation op)
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

        public IQueryable<T> List => _query(_includeOptions.List);

        public T Find(TKey Id)
        {
            return _query(_includeOptions.Find).SingleOrDefault(_ => _.Id.Equals(Id));
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

