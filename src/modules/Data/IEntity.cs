using System;
using System.Collections.Generic;
using System.Text;

namespace core.Extensions.Data
{
    public interface IEntity
    {
        Guid Id { get; set; }
    }
    public class Entity : IEntity
    {
        public virtual Guid Id { get; set; }
    }
}