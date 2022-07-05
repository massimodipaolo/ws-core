using Xunit;
using Xunit.Abstractions;

namespace xCore
{
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
        [InlineData(typeof(Ws.Core.Extensions.Data.Cache.ICache<xCore.Log>), typeof(Ws.Core.Extensions.Data.Cache.Memcached.MemcachedCache<xCore.Log>))]
        public void Check_RepositoryType(Type Tinterface, Type ExpectedTimplementation)
            => base.Check_ServiceImplementation(Tinterface, ExpectedTimplementation);

        [Theory]
        [InlineData(typeof(Models.CrudBase1))]
        [InlineData(typeof(Models.CrudBase2))]
        [InlineData(typeof(Models.CrudBase3))]
        [InlineData(typeof(xCore.Log))]
        public async Task Get(Type type) {
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
                new[] { xCore.Endpoints.Cache.Key(type), xCore.Endpoints.Cache.Key(type,id) }.All(_ => keys["prev"].Contains(_))
                && keys["current"].Count() == 1 
                && keys["current"].Contains(xCore.Endpoints.Cache.Key(type))
                );
        }
    }
}
