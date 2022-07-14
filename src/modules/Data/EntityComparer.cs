using System;
using System.Collections.Generic;
using System.Text;

namespace Ws.Core.Extensions.Data
{
    public class EntityComparer<T, TKey> : IEqualityComparer<T> where T : IEntity<TKey> where TKey : IEquatable<TKey>, IComparable<TKey>
    {
        public bool Equals(T? x, T? y)
        {
            if (x == null || x.Id == null || y == null || y.Id == null) return false;
            return x.Id.Equals(y.Id);
        }

        public int GetHashCode(T obj)
        {
            if (obj == null || obj.Id == null) return 0;
            return obj.Id.GetHashCode();
        }
    }
}
