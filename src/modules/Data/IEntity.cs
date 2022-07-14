using System;
using System.Collections.Generic;
using System.Text;

namespace Ws.Core.Extensions.Data
{

    public interface IEntity
    {  
    }

    public interface IEntity<TKey> :IEntity where TKey : IEquatable<TKey>, IComparable<TKey>
    {
        TKey? Id { get; set; } 
    }

    public class Entity<TKey> : IEntity<TKey> where TKey : IEquatable<TKey>, IComparable<TKey>
    {
        public virtual TKey? Id { get; set; } 
    }
}