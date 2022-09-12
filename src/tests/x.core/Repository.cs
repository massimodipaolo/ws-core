using Enyim.Caching.Memcached;
using System.Text.Json;
using Ws.Core.Extensions.Data;
using x.core.Models;
using Xunit;
using Xunit.Abstractions;

namespace x.core;

public class AppEmbeddedDbContextExt : Ws.Core.Extensions.Data.EF.SQLite.DbContext
{
    public AppEmbeddedDbContextExt(Microsoft.EntityFrameworkCore.DbContextOptions<Ws.Core.Extensions.Data.EF.SQLite.DbContext> options, ILogger<AppEmbeddedDbContextExt> logger) : base(options, logger) { }

    protected override void OnModelCreating(Microsoft.EntityFrameworkCore.ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Ignore<FakeEntity>();
    }

    public class FakeEntity: Ws.Core.Extensions.Data.Entity<int> { }
}

public class Repository : BaseTest
{
    Repository(Program factory) : base(factory) { }
    public Repository(Program factory, ITestOutputHelper output) : base(factory, output) {}

    [Theory]
    // default
    [InlineData(typeof(Ws.Core.Extensions.Data.IRepository<Models.User,int>), typeof(Ws.Core.Extensions.Data.Repository.EF.SqlServer<Models.User, int>))]
    [InlineData(typeof(Ws.Core.Extensions.Data.IRepository<Models.CrudBase1, Guid>), typeof(Ws.Core.Extensions.Data.Repository.EF.SqlServer<Models.CrudBase1, Guid>))]
    [InlineData(typeof(Ws.Core.Extensions.Data.IRepository<Models.CrudBase2, Guid>), typeof(Ws.Core.Extensions.Data.Repository.EF.SqlServer<Models.CrudBase2, Guid>))]
    // override on startup
    [InlineData(typeof(Ws.Core.Extensions.Data.IRepository<Models.Agenda, string>), typeof(Ws.Core.Extensions.Data.Repository.EF.MySql<Models.Agenda, string>))]
    [InlineData(typeof(Ws.Core.Extensions.Data.IRepository<Models.CrudBase3, Guid>), typeof(Ws.Core.Extensions.Data.Repository.Mongo<Models.CrudBase3, Guid>))]
    // override by injector
    [InlineData(typeof(Ws.Core.Extensions.Data.IRepository<Log, int>), typeof(Ws.Core.Extensions.Data.Repository.EF.SQLite<Log, int>))]
    [InlineData(typeof(Ws.Core.Extensions.Data.IRepository<Models.User2, int>), typeof(Ws.Core.Extensions.Data.Repository.FileSystem<Models.User2, int>))]
    [InlineData(typeof(Ws.Core.Extensions.Data.IRepository<Models.CrudBase, string>), typeof(Ws.Core.Extensions.Data.Repository.InMemory<Models.CrudBase, string>))]
    public void Check_RepositoryType(Type Tinterface, Type ExpectedTimplementation)
        => base.Check_ServiceImplementation(Tinterface, ExpectedTimplementation);

    [Fact]
    public async Task Check_InMemoryCrudOp() {        
        if (_getService(typeof(Ws.Core.Extensions.Data.IRepository<Models.CrudBase, string>)) is Ws.Core.Extensions.Data.IRepository<Models.CrudBase, string> _repo)
        {
            _repo.AddMany(Enumerable.Range(0, 10).Select(_ => new CrudBase() { }));
            await Check_CrudOp<Models.CrudBase, string>();
        }
        else
            Assert.True(false);
    }

    [Fact]
    public async Task Check_FileSystemCrudOp() => await Check_CrudOp<Models.User2, int>();
    [Fact]
    public async Task Check_SqlServerCrudOp() => await Check_CrudOp<Models.CrudBase1, Guid>();
    [Fact]
    public async Task Check_SqlServerStoredProcedureCrudOp() => await Check_CrudOp<Models.CrudBase2, Guid>();
    [Fact]
    public async Task Check_MongoCrudOp() => await Check_CrudOp<Models.CrudBase3, Guid>();
    [Fact]
    public async Task Check_MySqlCrudOp() => await Check_CrudOp<Models.Agenda, string>();

    protected async Task Check_CrudOp<T, TKey>() where T : IRecord, IEntity<TKey>, IAppTracked, new() where TKey : IEquatable<TKey>, IComparable<TKey>
    {
        // Assert
        var _prefix = $"/api/{nameof(x.core.Endpoints.App).ToLower()}/{typeof(T).Name}";
        var maxItems = 1000;
        // Get
        var getAll = await getAll<T,TKey>(_prefix,maxItems);
        Assert.True(getAll.response.IsSuccessStatusCode);
        List<T> entities = getAll.entities.ToList();
        Assert.True(entities?.Any() == true);
        var entitiesCount = entities?.Count;

        if (entities != null && entities?.Any() == true)
        {
            // Get Id
            var getId = await Get_EndpointsResponse($"{_prefix}/{entities?.First().Id?.ToString() ?? ""}");
            Assert.True(getId.response.IsSuccessStatusCode);
            var getIdCurrent = JsonSerializer.Deserialize<T>(getId.content);
            if (getIdCurrent == null)
            {
                Assert.True(false);
                return;
            }

            // Put
            var newTrackingDate = DateTime.Now;
            var toPut = getIdCurrent with { CreatedAt = newTrackingDate };
            var putId = await Put_EndpointsResponse($"{_prefix}/{getIdCurrent.Id}", _httpContent(toPut));
            Assert.True(putId.response.IsSuccessStatusCode);
            var getPutId = await Get_EndpointsResponse($"{_prefix}/{toPut.Id}");
            var getIdUpdated = JsonSerializer.Deserialize<T>(getPutId.content);
            Assert.True(getIdUpdated?.CreatedAt.ToUniversalTime().ToString("yyyyMMddHHmmss") == newTrackingDate.ToUniversalTime().ToString("yyyyMMddHHmmss"));

            // Post
            var toPost = getIdCurrent with { Id = new T().Id, CreatedAt = DateTime.Now };
            var post = await Post_EndpointsResponse($"{_prefix}", _httpContent(toPost));
            Assert.True(post.response.IsSuccessStatusCode);

            // Delete
            var getLastPost = await getAll<T, TKey>(_prefix, 1);
            if (getLastPost.entities.FirstOrDefault() is T _lastPost)
            {
                var deleteId = await Delete_EndpointsResponse($"{_prefix}/{_lastPost.Id}");
                Assert.True(deleteId.response.IsSuccessStatusCode);
            }

            // Put Many
            var toPutMany = await getAll<T, TKey>(_prefix, 5);
            var putMany = await Put_EndpointsResponse($"{_prefix}", _httpContent(toPutMany.entities));
            Assert.True(putMany.response.IsSuccessStatusCode);

            // Post Many
            var toPostMany = toPutMany.entities.Select(_ => _ with { Id = new T().Id, CreatedAt = DateTime.Now }).ToList();
            var postMany = await Post_EndpointsResponse($"{_prefix}/range", _httpContent(toPostMany));
            Assert.True(postMany.response.IsSuccessStatusCode);

            // Delete Many
            var getLastPostMany = await getAll<T, TKey>(_prefix, 5);
            var deleteMany = await DeleteMany_EndpointsResponse($"{_prefix}", _httpContent(getLastPostMany.entities));
            Assert.True(deleteMany.response.IsSuccessStatusCode);

            // Merge upsert
            var getMergeUpsert = await getAll<T, TKey>(_prefix, 5);
            var toMergeUpsert = getMergeUpsert.entities.Take(4)
                .Select(_ => _ with { CreatedAt = DateTime.Now }) // update
                .Union(getMergeUpsert.entities.Reverse().Take(1).Select(_ => _ with { Id = new T().Id, CreatedAt = DateTime.Now }))
                .ToList();
            var mergeUpsert = await Post_EndpointsResponse($"{_prefix}/merge/{RepositoryMergeOperation.Upsert}", _httpContent(toMergeUpsert));
            Assert.True(mergeUpsert.response.IsSuccessStatusCode);

            // Merge sync (with init getAll) 
            var mergeSync = await Post_EndpointsResponse($"{_prefix}/merge/{RepositoryMergeOperation.Sync}", _httpContent(entities ?? Array.Empty<T>().ToList()));
            Assert.True(mergeSync.response.IsSuccessStatusCode);

            // Final check
            var getAllAfterSync = await getAll<T, TKey>(_prefix, maxItems);
            IEnumerable<T> entitiesAfterSync = getAllAfterSync.entities;
            Assert.True(
                entitiesAfterSync?.Any() == true 
                && entitiesCount == entitiesAfterSync.Count() 
                && _isEquals<T, TKey>(entities ?? Array.Empty<T>().ToList()
                , getAllAfterSync.entities));
        }
        else
            Assert.True(false);

    }

    private static HttpContent _httpContent<Tobj>(Tobj obj) => new StringContent(
        JsonSerializer.Serialize<Tobj>(obj),
        System.Text.Encoding.UTF8,
        "application/json");

    private async Task<(HttpResponseMessage response, string content, IEnumerable<T> entities)> getAll<T, TKey>(string _prefix, int maxItems)
        where T : IRecord, IEntity<TKey>, IAppTracked, new() where TKey : IEquatable<TKey>, IComparable<TKey>
    {
        var (response, content) = await Get_EndpointsResponse(_prefix);
        IEnumerable<T> entities = JsonSerializer.Deserialize<IEnumerable<T>>(content)?.OrderByDescending(_ => _.CreatedAt).ThenBy(_ => _.Id)?.Take(maxItems)?.ToList() ?? Array.Empty<T>().ToList();
        return (response, content, entities);
    }

    private static bool _isEquals<T, TKey>(IEnumerable<T> obj1, IEnumerable<T> obj2)
    where T : IRecord, IEntity<TKey>, IAppTracked, new() where TKey : IEquatable<TKey>, IComparable<TKey>
    {
        Func<IEnumerable<T>, int> _hash = (obj) => string.Concat(obj.OrderByDescending(_ => _.CreatedAt).ThenBy(_ => _.Id).Select(_ => $"{_.Id}-{_.CreatedAt.ToUniversalTime().ToString("yyyyMMddHHmmss")}|")).GetHashCode();
        return _hash(obj1) == _hash(obj2);
    }
}
