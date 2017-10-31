using System;
using System.Collections.Generic;
using System.Text;

namespace core.Extensions.Data
{
    public interface IEntity
    {
        string Id { get; set; }
    }
    public class Entity : IEntity
    {
        public virtual string Id { get; set; }
    }
}