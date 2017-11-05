using System;
using core.Extensions.Data;
namespace core.Extensions.Cache
{
    public interface ICachedRepository<T> : IRepository<T> where T : IEntity
    {
    }
}
