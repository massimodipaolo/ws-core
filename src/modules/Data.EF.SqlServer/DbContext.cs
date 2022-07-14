using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;

namespace Ws.Core.Extensions.Data.EF.SqlServer;

public class DbConnectionFunctionWrapper : Data.IDbConnectionFunctionWrapper
{
    public Func<Type, Data.DbConnection> Func { get; set; } = (t) => new Data.DbConnection() { };
}

public class DbContext : EF.DbContext<DbContext>
{    
    public DbContext(DbContextOptions<DbContext> options, ILogger<DbContext> logger) : base(options, logger) { }
}
