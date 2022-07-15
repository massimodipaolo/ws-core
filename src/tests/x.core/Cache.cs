using Ws.Core.Extensions.Data.Cache;
using Xunit;
using Xunit.Abstractions;

namespace x.core;

public class Cache : BaseTest
{
    public Cache(Program factory, ITestOutputHelper output) : base(factory, output) { }

    [Theory]
    // default
    [InlineData(typeof(Ws.Core.Extensions.Data.Cache.ICache), typeof(Ws.Core.Extensions.Data.Cache.MemoryCache))]
    [InlineData(typeof(Ws.Core.Extensions.Data.Cache.ICache<Models.CrudBase1>), typeof(Ws.Core.Extensions.Data.Cache.MemoryCache<Models.CrudBase1>))]                         
    // override on startup
    [InlineData(typeof(Ws.Core.Extensions.Data.Cache.ICache<Models.CrudBase2>), typeof(Ws.Core.Extensions.Data.Cache.SqlServer.SqlCache<Models.CrudBase2>))]
    // override by injector
    [InlineData(typeof(Ws.Core.Extensions.Data.Cache.ICache<Models.CrudBase3>), typeof(Ws.Core.Extensions.Data.Cache.Redis.RedisCache<Models.CrudBase3>))]
    [InlineData(typeof(Ws.Core.Extensions.Data.Cache.ICache<x.core.Log>), typeof(Ws.Core.Extensions.Data.Cache.Memcached.MemcachedCache<x.core.Log>))]
    public void Check_RepositoryType(Type Tinterface, Type ExpectedTimplementation)
        => base.Check_ServiceImplementation(Tinterface, ExpectedTimplementation);

    [Theory]
    [InlineData(typeof(Models.CrudBase1))]
    [InlineData(typeof(Models.CrudBase2))]
    [InlineData(typeof(Models.CrudBase3))]
    [InlineData(typeof(x.core.Log))]
    public async Task Check_CrudOp(Type type) {
        var url = $"/api/cache/{type.Name}".ToLower();
        // flush
        var (rsDelete, contentDelete) = await Delete_EndpointsResponse(url);
        Assert.True(rsDelete.IsSuccessStatusCode);
        // first call
        var (rsGet1, contentGet1) = await Get_EndpointsResponse(url);
        Assert.True(rsGet1.IsSuccessStatusCode);
        Assert.True(string.IsNullOrEmpty(contentGet1));
        // second call
        var (rsGet2, contentGet2) = await Get_EndpointsResponse(url);
        Assert.True(!string.IsNullOrEmpty(contentGet2));
        // remove
        object id;
        try
        {
            id = ((Ws.Core.Extensions.Data.Entity<Guid>[])System.Text.Json.JsonSerializer.Deserialize(contentGet2, typeof(Ws.Core.Extensions.Data.Entity<Guid>[]))).FirstOrDefault().Id;
        } catch
        {
            id = ((Ws.Core.Extensions.Data.Entity<int>[])System.Text.Json.JsonSerializer.Deserialize(contentGet2, typeof(Ws.Core.Extensions.Data.Entity<int>[]))).FirstOrDefault().Id;
        }            
        var (rsDelete1, contentDelete1) = await Delete_EndpointsResponse($"{url}/{id}");
        Assert.True(rsDelete1.IsSuccessStatusCode);
        Dictionary<string, string[]> keys = (Dictionary<string, string[]>)System.Text.Json.JsonSerializer.Deserialize(contentDelete1, typeof(Dictionary<string, string[]>));
        Assert.True(
            new[] { x.core.Endpoints.Cache.Key(type), x.core.Endpoints.Cache.Key(type,id) }.All(_ => keys["prev"].Contains(_))
            && keys["current"].Length == 1 
            && keys["current"].Contains(x.core.Endpoints.Cache.Key(type))
            );
    }

    [Theory]
    // MemoryCache
    [InlineData(typeof(ICache), new double[] { 1, 5, 60, 1440 })]
    // SqlCache
    [InlineData(typeof(ICache<Models.CrudBase2>), new double[] { 2, 10, 120, 2880 })]
    // RedisCache
    [InlineData(typeof(ICache<Models.CrudBase3>), new double[] { 10, 60, 240, 1440 })]
    // MemcachedCache
    [InlineData(typeof(ICache<x.core.Log>), new double[] { 10, 60, 240, 1440 })]
    public void Check_ExpirationTier(Type tCache, double[] minutes)
    {
        var cache = (ICache)_factory.Services.GetRequiredService(tCache);
        _output.WriteLine($"{cache.GetType().FullName} - {cache.ExpirationTier.Fast.AbsoluteExpirationRelativeToNow}");
        Assert.Equal(ExpirationTier.NoCacheValue.AbsoluteExpirationRelativeToNow, cache.ExpirationTier.NoCache.AbsoluteExpirationRelativeToNow);
        Assert.Equal(TimeSpan.FromMinutes(minutes[0]), cache.ExpirationTier.Fast.AbsoluteExpirationRelativeToNow);
        Assert.Equal(TimeSpan.FromMinutes(minutes[1]), cache.ExpirationTier.Medium.AbsoluteExpirationRelativeToNow);
        Assert.Equal(TimeSpan.FromMinutes(minutes[2]), cache.ExpirationTier.Slow.AbsoluteExpirationRelativeToNow);
        Assert.Equal(TimeSpan.FromMinutes(minutes[3]), cache.ExpirationTier.Never.AbsoluteExpirationRelativeToNow);
    }
}
