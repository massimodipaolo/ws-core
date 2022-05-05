using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ws.Core.Extensions.Data.EF.MySql;

public class DbConnectionFunctionWrapper: Data.IDbConnectionFunctionWrapper
{
    public Func<Type, Data.DbConnection> Func { get; set; }
}
public class DbContext : EF.DbContext<DbContext>
{
    public DbContext(DbContextOptions<DbContext> options) : base(options) { }
}