using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ws.Core.Extensions.Data.EF.MySql;


public class DbContext : EF.DbContext<MySql.DbContext>
{
    static DbContext() {}
    public DbContext(DbContextOptions<MySql.DbContext> options) : base(options) { }
}


