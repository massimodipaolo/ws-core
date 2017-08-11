using System;

namespace core.Extensions.Data
{
    public interface IStorage<T> where T : IEntity
    {
        string Connection { get; }
        IRepository<T> Repository { get; set; }
    }
}
