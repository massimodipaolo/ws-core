using Microsoft.EntityFrameworkCore;
using System;

namespace Ws.Core.Extensions.Data.EF.SQLite;

public class DbConnectionFunctionWrapper : Data.IDbConnectionFunctionWrapper
{
    public Func<Type, Data.DbConnection> Func { get; set; }
}
public class DbContext : EF.DbContext<DbContext>
{
    public DbContext(DbContextOptions<SQLite.DbContext> options) : base(options) { }
}

public class DbContext<TContext> : EF.DbContext where TContext : EF.DbContext
{
    public DbContext(DbContextOptions<TContext> options) : base(options) { }
}

