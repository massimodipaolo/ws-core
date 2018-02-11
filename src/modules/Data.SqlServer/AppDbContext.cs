using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace core.Extensions.Data
{
    public class AppDbContext : DbContext
    {
        static AppDbContext()
        {

        }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (Type type in Base.Util.GetAllTypesOf<IEntity>().Where(_ => _ != typeof(Entity)))
            {
                try
                {
                    modelBuilder.Entity(type)
                                .ToTable(type.Name)
                                .Property("Id").HasColumnName("Id").HasColumnType("uniqueidentifier").HasDefaultValue()
                                ;
                }
                catch { }
            }

        }



    }
}
