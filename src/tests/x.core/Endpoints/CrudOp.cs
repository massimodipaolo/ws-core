using Ws.Core.Extensions.Data;

namespace x.core.Endpoints;

public class CrudOp
{
    protected CrudOp() { }
    protected static IEnumerable<T> GetAll<T, TKey>(IRepository<T, TKey> repo) where T : class, IEntity<TKey> where TKey : IEquatable<TKey>, IComparable<TKey> => repo.List;
    protected static IResult GetById<T, TKey>(TKey id, IRepository<T, TKey> repo) where T : class, IEntity<TKey> where TKey : IEquatable<TKey>, IComparable<TKey> =>
        repo.Find(id) is T entity
                ? Results.Ok(entity)
                : Results.NotFound();
    protected static IResult Create<T, TKey>(T entity, IRepository<T, TKey> repo, ILogger<T> logger) where T : class, IEntity<TKey> where TKey : IEquatable<TKey>, IComparable<TKey>
    {
        repo.Add(entity);
        logger.LogInformation("Obj of type {T} created: {@entity}", typeof(T), entity);
        return Results.Created($"/{typeof(T)}/{entity.Id}", entity);
    }

    protected static IResult CreateMany<T, TKey>([Microsoft.AspNetCore.Mvc.FromBody] IEnumerable<T> entities, IRepository<T, TKey> repo) where T : class, IEntity<TKey> where TKey : IEquatable<TKey>, IComparable<TKey>
    {
        repo.AddMany(entities);
        return Results.NoContent();
    }

    protected static IResult Delete<T, TKey>([Microsoft.AspNetCore.Mvc.FromRoute] TKey id, IRepository<T, TKey> repo, ILogger<T> logger) where T : class, IEntity<TKey> where TKey : IEquatable<TKey>, IComparable<TKey>
    {
        if (repo.Find(id) is T item)
        {
            repo.Delete(item);
            logger.LogInformation("Obj of type {T} deleted: {@item}", typeof(T), item);
            return Results.NoContent();
        }
        else
            return Results.NotFound();
    }
    protected static IResult DeleteMany<T, TKey>([Microsoft.AspNetCore.Mvc.FromBody] IEnumerable<T> entities, IRepository<T, TKey> repo) where T : class, IEntity<TKey> where TKey : IEquatable<TKey>, IComparable<TKey>
    {
        repo.DeleteMany(entities);
        return Results.NoContent();
    }
    protected static IResult Update<T, TKey>(TKey id, [Microsoft.AspNetCore.Mvc.FromBody] T entity, IRepository<T,TKey> repo) where T : class, IEntity<TKey> where TKey : IEquatable<TKey>, IComparable<TKey>
    {
        if (id.Equals(entity.Id) && repo.Find(id) is not null)
        {
            repo.Update(entity);
            return Results.NoContent();
        }
        else
            return Results.NotFound();
    }

    protected static IResult UpdateMany<T, TKey>([Microsoft.AspNetCore.Mvc.FromBody] IEnumerable<T> entities, IRepository<T, TKey> repo) where T : class, IEntity<TKey> where TKey : IEquatable<TKey>, IComparable<TKey>
    {
        repo.UpdateMany(entities);
        return Results.NoContent();
    }

    protected static IResult Merge<T, TKey>([Microsoft.AspNetCore.Mvc.FromRoute] RepositoryMergeOperation? operation, [Microsoft.AspNetCore.Mvc.FromBody]IEnumerable<T> entities, IRepository < T, TKey> repo) where T : class, IEntity<TKey> where TKey : IEquatable<TKey>, IComparable<TKey>
    {
        repo.Merge(entities, operation ?? RepositoryMergeOperation.Upsert);
        return Results.NoContent();
    }
}
