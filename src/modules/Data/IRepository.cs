using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ws.Core.Extensions.Data
{
    public interface IRepository<T, TKey> where T : class, IEntity<TKey> where TKey : IEquatable<TKey>
    {
        IQueryable<T> List { get; }
        T Find(TKey Id);
        IQueryable<T> Query(FormattableString command);
        void Add(T entity);
        void AddMany(IEnumerable<T> entities);
        void Update(T entity);
        void UpdateMany(IEnumerable<T> entities);
        void Merge(IEnumerable<T> entities, RepositoryMergeOperation operation = RepositoryMergeOperation.Upsert);
        void Delete(T entity);
        void DeleteMany(IEnumerable<T> entities);
    }

    [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
    public enum RepositoryMergeOperation
    {
        Upsert,
        Sync
    }
}
