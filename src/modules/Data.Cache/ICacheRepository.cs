using System;

namespace core.Extensions.Data.Cache
{
    public interface ICacheRepository<T, TKey> : IRepository<T, TKey> where T : IEntity<TKey> where TKey : IEquatable<TKey>
    {
    }
}
