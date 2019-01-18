using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace core.Extensions.Data
{
    public interface IRepository<T, TKey> where T : class, IEntity<TKey> where TKey : IEquatable<TKey>
    {
        IQueryable<T> List { get; }
        T Find(TKey Id);
        IQueryable<T> Query(FormattableString command);
        void Add(T entity);
        void Update(T entity);
        void Merge(IEnumerable<T> entities, RepositoryMergeOperation operation = RepositoryMergeOperation.Upsert);
        void Delete(T entity);
    }

    public enum RepositoryMergeOperation
    {
        Upsert,
        Sync
    }
}
