namespace core.Extensions.Data.Cache
{
    public interface ICachedRepository<T> : IRepository<T> where T : IEntity
    {
    }
}
