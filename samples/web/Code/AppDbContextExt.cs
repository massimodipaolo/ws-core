using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ws.Core.Extensions.Data;

namespace web.Code
{
    public class AppDbContextExt : AppDbContext
    {
        static AppDbContextExt()
        {

        }
        public AppDbContextExt(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
