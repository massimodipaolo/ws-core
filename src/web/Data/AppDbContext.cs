using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace web.Data
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
            #warning get all type by reflection: https://gist.github.com/jcansdale/f0c2f70c2dc8094e4fb8eaba6506a550
            foreach (Type type in (new Type[] { typeof(web.Models.User) }))
                modelBuilder.Entity(type); //.ToTable(type.Name);
            // new MyClassMap(modelBuilder.Entity<MyClass>());            
        }
    }
}
