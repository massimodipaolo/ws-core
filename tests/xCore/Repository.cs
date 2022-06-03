using Xunit;
using Xunit.Abstractions;

namespace xCore;

public class AppEmbeddedDbContextExt : Ws.Core.Extensions.Data.EF.SQLite.DbContext
{
    public AppEmbeddedDbContextExt(Microsoft.EntityFrameworkCore.DbContextOptions<Ws.Core.Extensions.Data.EF.SQLite.DbContext> options) : base(options) { }

    protected override void OnModelCreating(Microsoft.EntityFrameworkCore.ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        //modelBuilder.Ignore<xCore.Log>();
    }
}

public class Repository : BaseTest
{
    public Repository(Program factory, ITestOutputHelper output) : base(factory, output) {}

    [Theory]
    // default
    [InlineData(typeof(Ws.Core.Extensions.Data.IRepository<Models.User,int>), typeof(Ws.Core.Extensions.Data.Repository.EF.SqlServer<Models.User, int>))]
    // override on startup
    [InlineData(typeof(Ws.Core.Extensions.Data.IRepository<Endpoints.Agenda, string>), typeof(Ws.Core.Extensions.Data.Repository.EF.MySql<Endpoints.Agenda, string>))]
    // override by injector
    [InlineData(typeof(Ws.Core.Extensions.Data.IRepository<Log, int>), typeof(Ws.Core.Extensions.Data.Repository.EF.SQLite<Log, int>))]
    [InlineData(typeof(Ws.Core.Extensions.Data.IRepository<Models.User2, int>), typeof(Ws.Core.Extensions.Data.Repository.FileSystem<Models.User2, int>))]
    public void Check_RepositoryType(Type Tinterface, Type ExpectedTimplementation)
        => base.Check_ServiceImplementation(Tinterface, ExpectedTimplementation);
}
