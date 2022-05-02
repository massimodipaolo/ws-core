using Microsoft.EntityFrameworkCore;

namespace Ws.Core.Extensions.Data.EF.SQLite;


public class DbContext : EF.DbContext<DbContext>
{
    static DbContext() {}
    public DbContext(DbContextOptions<SQLite.DbContext> options) : base(options) { }
}


