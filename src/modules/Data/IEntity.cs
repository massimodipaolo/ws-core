using System;
using System.Collections.Generic;
using System.Text;

namespace core.Extensions.Data
{
    public interface IEntity<TKey>
    {
        TKey Id { get; set; }
    }

    public class Entity<TKey> : IEntity<TKey> where TKey : IEquatable<TKey>
    {
        public virtual TKey Id { get; set; }
    }

}