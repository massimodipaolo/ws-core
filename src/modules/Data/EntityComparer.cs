using System;
using System.Collections.Generic;
using System.Text;

namespace Ws.Core.Extensions.Data
{
    public class EntityComparer<T, TKey> : IEqualityComparer<T> where T : IEntity<TKey> where TKey : IEquatable<TKey>
    {
        public bool Equals(T x, T y)
        {
            return x.Id.Equals(y.Id);
        }

        public int GetHashCode(T obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
