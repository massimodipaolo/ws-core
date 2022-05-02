using Ws.Core.Extensions.Data;

namespace xCore.Endpoints;

public class CrudOp
{
    protected static IEnumerable<T> GetAll<T, TKey>(IRepository<T, TKey> _repo) where T : class, IEntity<TKey> where TKey : IEquatable<TKey> => _repo.List;
    protected static IResult GetById<T, TKey>(TKey id, IRepository<T, TKey> _repo) where T : class, IEntity<TKey> where TKey : IEquatable<TKey> =>
        _repo.Find(id) is T item
                ? Results.Ok(item)
                : Results.NotFound();
    protected static IResult Create<T, TKey>(T item, IRepository<T, TKey> _repo) where T : class, IEntity<TKey> where TKey : IEquatable<TKey>
    {
        _repo.Add(item);
        return Results.Created($"/{typeof(T)}/{item.Id}", item);
    }
    protected static IResult Delete<T, TKey>(TKey id, IRepository<T, TKey> _repo) where T : class, IEntity<TKey> where TKey : IEquatable<TKey>
    {
        if (_repo.Find(id) is T item)
        {
            _repo.Delete(item);
            return Results.NoContent();
        }
        else
            return Results.NotFound();
    }
    protected static IResult Update<T, TKey>(TKey id, [Microsoft.AspNetCore.Mvc.FromBody] T item, IRepository<T,TKey> _repo) where T : class, IEntity<TKey> where TKey : IEquatable<TKey>
    {
        if (id.Equals(item.Id) && _repo.Find(id) is T find)
        {
            _repo.Update(item);
            return Results.NoContent();
        }
        else
            return Results.NotFound();
    }
}
