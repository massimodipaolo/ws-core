using core.Extensions.Data.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace core.Extensions.Data.Repository.EF
{
    /// <summary>
    /// https://stackoverflow.com/a/43928098/11074305 by https://stackoverflow.com/users/488397/steve 
    /// </summary>
    public static class Extensions
    {
        public static IIncludableJoin<TEntity, TProperty> IncludeJoin<TEntity, TProperty>(
            this IQueryable<TEntity> query,
            Expression<Func<TEntity, TProperty>> propToExpand)
            where TEntity : class
        {
            return new IncludableJoin<TEntity, TProperty>(query.Include(propToExpand));
        }

        public static IIncludableJoin<TEntity, TProperty> IncludeJoin<TEntity, TProperty>(
            this ICollection<TEntity> collection,
            Expression<Func<TEntity, TProperty>> propToExpand)
            where TEntity : class
        {
            return new IncludableJoin<TEntity, TProperty>(collection.AsQueryable().Include(propToExpand));
        }

        public static IIncludableJoin<TEntity, TProperty> ThenIncludeJoin<TEntity, TPreviousProperty, TProperty>(
             this IIncludableJoin<TEntity, TPreviousProperty> query,
             Expression<Func<TPreviousProperty, TProperty>> propToExpand)
            where TEntity : class
        {
            IIncludableQueryable<TEntity, TPreviousProperty> queryable = ((IncludableJoin<TEntity, TPreviousProperty>)query).GetQuery();
            return new IncludableJoin<TEntity, TProperty>(queryable.ThenInclude(propToExpand));
        }

        public static IIncludableJoin<TEntity, TProperty> ThenIncludeJoin<TEntity, TPreviousProperty, TProperty>(
            this IIncludableJoin<TEntity, IEnumerable<TPreviousProperty>> query,
            Expression<Func<TPreviousProperty, TProperty>> propToExpand)
            where TEntity : class
        {
            var queryable = ((IncludableJoin<TEntity, IEnumerable<TPreviousProperty>>)query).GetQuery();
            var include = queryable.ThenInclude(propToExpand);
            return new IncludableJoin<TEntity, TProperty>(include);
        }

        public static IIncludableJoin<TEntity, TProperty> ThenIncludeJoin<TEntity, TPreviousProperty, TProperty>(
            this IIncludableJoin<TEntity, ICollection<TPreviousProperty>> query,
            Expression<Func<TPreviousProperty, TProperty>> propToExpand)
            where TEntity : class
        {
            var queryable = ((IncludableJoin<TEntity, ICollection<TPreviousProperty>>)query).GetQuery();
            var include = queryable.ThenInclude(propToExpand);
            return new IncludableJoin<TEntity, TProperty>(include);
        }

    }
}
