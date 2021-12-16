using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ws.Core.Extensions.Data
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> List { get; }
        IQueryable<T> Query(FormattableString command);
        void Add(T entity);
        void AddMany(IEnumerable<T> entities);
        void Update(T entity);
        void UpdateMany(IEnumerable<T> entities);
        void Merge(IEnumerable<T> entities, RepositoryMergeOperation operation = RepositoryMergeOperation.Upsert);
        void Delete(T entity);
        void DeleteMany(IEnumerable<T> entities);
    }
    public interface IRepository<T, TKey> : IRepository<T> where T : class, IEntity<TKey> where TKey : IEquatable<TKey>
    {
        T Find(TKey Id);
    }

    [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
    public enum RepositoryMergeOperation
    {
        Upsert,
        Sync
    }
}
