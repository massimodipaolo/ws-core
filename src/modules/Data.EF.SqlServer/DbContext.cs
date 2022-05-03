using Microsoft.EntityFrameworkCore;
using System;

namespace Ws.Core.Extensions.Data.EF.SqlServer;

public class DbContextFunctionWrapper : EF.IDbContextFunctionWrapper
{
    public Func<Type, EF.DbContext> Func { get; set; }
}
public class DbContext : EF.DbContext<DbContext>
{    
    public DbContext(DbContextOptions<DbContext> options) : base(options) { }
}

public class DbContext<TContext> : EF.DbContext where TContext : EF.DbContext
{
    public DbContext(DbContextOptions<TContext> options) : base(options) { }
}
public class DbContext0 : DbContext<DbContext0>
{
    public DbContext0(DbContextOptions<DbContext0> options) : base(options) {}
}
public class DbContext1 : DbContext<DbContext1>
{
    public DbContext1(DbContextOptions<DbContext1> options) : base(options) { }
}
public class DbContext2 : DbContext<DbContext2>
{
    public DbContext2(DbContextOptions<DbContext2> options) : base(options) { }
}
public class DbContext3 : DbContext<DbContext3>
{
    public DbContext3(DbContextOptions<DbContext3> options) : base(options) { }
}
public class DbContext4 : DbContext<DbContext4>
{
    public DbContext4(DbContextOptions<DbContext4> options) : base(options) { }
}
public class DbContext5 : DbContext<DbContext5>
{
    public DbContext5(DbContextOptions<DbContext5> options) : base(options) { }
}
public class DbContext6 : DbContext<DbContext6>
{
    public DbContext6(DbContextOptions<DbContext6> options) : base(options) { }
}
public class DbContext7 : DbContext<DbContext7>
{
    public DbContext7(DbContextOptions<DbContext7> options) : base(options) { }
}
public class DbContext8 : DbContext<DbContext8>
{
    public DbContext8(DbContextOptions<DbContext8> options) : base(options) { }
}

