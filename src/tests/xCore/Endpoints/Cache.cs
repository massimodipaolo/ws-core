using Carter;
using Ws.Core.Extensions.Data;
using Ws.Core.Extensions.Data.Cache;
using xCore.Models;

namespace xCore.Endpoints;
public class Cache : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        // Memory
        app.MapGet($"api/cache/{nameof(xCore.Models.CrudBase1)}", GetOrCreate<xCore.Models.CrudBase1, Guid>);
        app.MapGet($"api/cache/{nameof(xCore.Models.CrudBase1)}/{{id}}", GetOrCreate<xCore.Models.CrudBase1, Guid>);
        app.MapDelete($"api/cache/{nameof(xCore.Models.CrudBase1)}/{{id}}", Remove<xCore.Models.CrudBase1, Guid>);
        app.MapDelete($"api/cache/{nameof(xCore.Models.CrudBase1)}", Flush<xCore.Models.CrudBase1>);
        // Sql
        app.MapGet($"api/cache/{nameof(xCore.Models.CrudBase2)}", GetOrCreate<xCore.Models.CrudBase2, Guid>);
        app.MapGet($"api/cache/{nameof(xCore.Models.CrudBase2)}/{{id}}", GetOrCreate<xCore.Models.CrudBase2, Guid>);
        app.MapDelete($"api/cache/{nameof(xCore.Models.CrudBase2)}/{{id}}", Remove<xCore.Models.CrudBase2, Guid>);
        app.MapDelete($"api/cache/{nameof(xCore.Models.CrudBase2)}", Flush<xCore.Models.CrudBase2>);
        // Redis
        app.MapGet($"api/cache/{nameof(xCore.Models.CrudBase3)}", GetOrCreate<xCore.Models.CrudBase3, Guid>);
        app.MapGet($"api/cache/{nameof(xCore.Models.CrudBase3)}/{{id}}", GetOrCreate<xCore.Models.CrudBase3, Guid>);
        app.MapDelete($"api/cache/{nameof(xCore.Models.CrudBase3)}/{{id}}", Remove<xCore.Models.CrudBase3, Guid>);
        app.MapDelete($"api/cache/{nameof(xCore.Models.CrudBase3)}", Flush<xCore.Models.CrudBase3>);
        // Memcached
        app.MapGet($"api/cache/{nameof(xCore.Log)}", GetOrCreate<xCore.Log, int>);
        app.MapGet($"api/cache/{nameof(xCore.Log)}/{{id}}", GetOrCreate<xCore.Log, int>);
        app.MapDelete($"api/cache/{nameof(xCore.Log)}/{{id}}", Remove<xCore.Log, int>);
        app.MapDelete($"api/cache/{nameof(xCore.Log)}", Flush<xCore.Log>);
    }
    private static string Key<T>() where T : class => Key(typeof(T));
    public static string Key(Type t)  => $"cache:{(typeof(xCore.Endpoints.Cache).FullName ?? typeof(xCore.Endpoints.Cache).Name)}:sample-{t.Name}".ToLower();
    private static string Key<T, TKey>(TKey id) where T : IRecord, IEntity<TKey>, IAppTracked, new() where TKey : IEquatable<TKey>, IComparable<TKey> 
        => Key(typeof(T), id);
    public static string Key(Type t, object id) => $"cache:{(typeof(xCore.Endpoints.Cache).FullName ?? typeof(xCore.Endpoints.Cache).Name)}:{t.Name}-{id}".ToLower() ?? "";
    public async Task<IResult> GetOrCreate<T, TKey>(ICache<T> cache, IRepository<T, TKey> repo) where T : IRecord, IEntity<TKey>, IAppTracked, new() where TKey : IEquatable<TKey>, IComparable<TKey>
    {
        var cached = await cache.GetAsync<IEnumerable<T>>(Key<T>());
        if (cached == null)
        {
            var list = repo.List.AsEnumerable().Take(5);
            await cache.SetObjectAsync(Key<T>(), list, CacheEntryOptions.Expiration.Medium);

            T? first = list.FirstOrDefault();
            if (first != null)
                await cache.SetAsync(Key<T, TKey>(first.Id), Ws.Core.Extensions.Data.Cache.Util.ObjToByte(first), CacheEntryOptions.Expiration.Fast);
        }
        return Results.Ok(cached);
    }
    public async Task<IResult> Remove<T, TKey>(ICache<T> cache, TKey id) where T : IRecord, IEntity<TKey>, IAppTracked, new() where TKey : IEquatable<TKey>, IComparable<TKey>
    {
        var prev = cache.Keys.ToList();
        await cache.RemoveAsync(Key<T, TKey>(id));
        return Results.Ok(new Dictionary<string, IEnumerable<string>>() { ["prev"] = prev, ["current"] = cache.Keys });
    }
    public async Task<IResult> Flush<T>(ICache<T> cache) where T : IRecord
    {
        await cache.ClearAsync();
        return Results.Ok();
    }
}
