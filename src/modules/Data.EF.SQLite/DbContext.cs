using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Ws.Core.Extensions.Data.EF.SQLite;

public class DbConnectionFunctionWrapper : Data.IDbConnectionFunctionWrapper
{
    public Func<Type, Data.DbConnection> Func { get; set; } = (t) => new Data.DbConnection() { };
}
public class DbContext : EF.DbContext<DbContext>
{
    public DbContext(DbContextOptions<SQLite.DbContext> options, ILogger<DbContext> logger) : base(options, logger) { }
}
