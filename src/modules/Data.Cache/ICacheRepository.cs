namespace core.Extensions.Data.Cache
{
    public interface ICacheRepository<T> : IRepository<T> where T : IEntity
    {
    }
}
