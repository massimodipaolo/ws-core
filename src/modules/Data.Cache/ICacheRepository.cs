using System;

namespace Ws.Core.Extensions.Data.Cache
{
    public interface ICacheRepository<T, TKey> : IRepository<T, TKey> where T : class, IEntity<TKey> where TKey : IEquatable<TKey>, IComparable<TKey>
    {
    }
}
