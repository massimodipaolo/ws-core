using Microsoft.EntityFrameworkCore;

namespace Ws.Core.Extensions.Data.EF.SqlServer;

public class DbContext : EF.DbContext<SqlServer.DbContext>
{
    static DbContext() {}
    public DbContext(DbContextOptions<SqlServer.DbContext> options) : base(options) { }
}


