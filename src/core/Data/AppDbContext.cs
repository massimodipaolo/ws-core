using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;

namespace core.Data
{
    public class AppDbContext : DbContext
    {
        static AppDbContext()
        {
            
        }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {            
            base.OnModelCreating(modelBuilder);            
            var baseType = typeof(Models.Entity);
            var entityTypes = core.Code.Utils.Assemblies.SelectMany(_ => _.GetTypes().Where(__ => baseType.IsAssignableFrom(__) && __!= baseType));
            foreach (Type type in entityTypes)
                modelBuilder.Entity(type); //.ToTable(type.Name);
            // new MyClassMap(modelBuilder.Entity<MyClass>());            
        }



    }
}
