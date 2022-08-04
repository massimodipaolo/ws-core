using Carter;
using Ws.Core.Extensions.Data;
using Ws.Core.Extensions.Data.Cache;
using x.core.Models;

namespace x.core.Endpoints;
public class Cache : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        // Memory
        app.MapGet($"api/cache/{nameof(x.core.Models.CrudBase1)}", GetOrCreate<x.core.Models.CrudBase1, Guid>);
        app.MapGet($"api/cache/{nameof(x.core.Models.CrudBase1)}/{{id}}", GetOrCreate<x.core.Models.CrudBase1, Guid>);
        app.MapDelete($"api/cache/{nameof(x.core.Models.CrudBase1)}/{{id}}", Remove<x.core.Models.CrudBase1, Guid>);
        app.MapDelete($"api/cache/{nameof(x.core.Models.CrudBase1)}", Flush<x.core.Models.CrudBase1>);
        // Sql
        app.MapGet($"api/cache/{nameof(x.core.Models.CrudBase2)}", GetOrCreate<x.core.Models.CrudBase2, Guid>);
        app.MapGet($"api/cache/{nameof(x.core.Models.CrudBase2)}/{{id}}", GetOrCreate<x.core.Models.CrudBase2, Guid>);
        app.MapDelete($"api/cache/{nameof(x.core.Models.CrudBase2)}/{{id}}", Remove<x.core.Models.CrudBase2, Guid>);
        app.MapDelete($"api/cache/{nameof(x.core.Models.CrudBase2)}", Flush<x.core.Models.CrudBase2>);
        // Redis
        app.MapGet($"api/cache/{nameof(x.core.Models.CrudBase3)}", GetOrCreate<x.core.Models.CrudBase3, Guid>);
        app.MapGet($"api/cache/{nameof(x.core.Models.CrudBase3)}/{{id}}", GetOrCreate<x.core.Models.CrudBase3, Guid>);
        app.MapDelete($"api/cache/{nameof(x.core.Models.CrudBase3)}/{{id}}", Remove<x.core.Models.CrudBase3, Guid>);
        app.MapDelete($"api/cache/{nameof(x.core.Models.CrudBase3)}", Flush<x.core.Models.CrudBase3>);
        // Memcached
        app.MapGet($"api/cache/{nameof(x.core.Log)}", GetOrCreate<x.core.Log, int>);
        app.MapGet($"api/cache/{nameof(x.core.Log)}/{{id}}", GetOrCreate<x.core.Log, int>);
        app.MapDelete($"api/cache/{nameof(x.core.Log)}/{{id}}", Remove<x.core.Log, int>);
        app.MapDelete($"api/cache/{nameof(x.core.Log)}", Flush<x.core.Log>);
    }
    private static string Key<T>() where T : class => Key(typeof(T));
    public static string Key(Type t)  => $"cache:{(typeof(x.core.Endpoints.Cache).FullName ?? typeof(x.core.Endpoints.Cache).Name)}:sample-{t.Name}".ToLower();
    private static string Key<T, TKey>(TKey id) where T : IRecord, IEntity<TKey>, IAppTracked, new() where TKey : IEquatable<TKey>, IComparable<TKey> 
        => Key(typeof(T), id);
    public static string Key(Type t, object id) => $"cache:{(typeof(x.core.Endpoints.Cache).FullName ?? typeof(x.core.Endpoints.Cache).Name)}:{t.Name}-{id}".ToLower() ?? "";
    public async Task<IResult> GetOrCreate<T, TKey>(ICache<T> cache, IRepository<T, TKey> repo) where T : IRecord, IEntity<TKey>, IAppTracked, new() where TKey : IEquatable<TKey>, IComparable<TKey>
    {
        var cached = await cache.GetAsync<IEnumerable<T>>(Key<T>());
        if (cached == null)
        {            
            var list = repo.List.AsEnumerable().Take(5);            
            await cache.SetObjectAsync(Key<T>(), list, cache.ExpirationTier.Medium);
            if (list.FirstOrDefault() is T first && first.Id != null)
                await cache.SetAsync(Key<T, TKey>(first.Id), Ws.Core.Extensions.Data.Cache.Util.ObjToByte(first), cache.ExpirationTier.Fast);
        }
        return Results.Ok(cached);
    }
    public async Task<IResult> Remove<T, TKey>(ICache<T> cache, TKey id) where T : IRecord, IEntity<TKey>, IAppTracked, new() where TKey : IEquatable<TKey>, IComparable<TKey>
    {
        var prev = cache.Keys.ToList();
        await cache.RefreshAsync(Key<T, TKey>(id));
        await cache.RemoveAsync(Key<T, TKey>(id));
        return Results.Ok(new Dictionary<string, IEnumerable<string>>() { ["prev"] = prev, ["current"] = cache.Keys });
    }
    public async Task<IResult> Flush<T>(ICache<T> cache) where T : IRecord
    {
        await cache.ClearAsync();
        return Results.Ok();
    }
}
