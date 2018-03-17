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

            var tKeys = new KeyValuePair<Type, string>[] {
                new KeyValuePair<Type,string>(typeof(IEntity<int>),"int"),
                new KeyValuePair<Type,string>(typeof(IEntity<long>),"bigint"),
                new KeyValuePair<Type, string>(typeof(IEntity<Guid>),"uniqueidentifier"),
                new KeyValuePair<Type, string>(typeof(IEntity<string>),"varchar")
            };

            foreach (KeyValuePair<Type, string> tKey in tKeys)
            {
                foreach (Type type in Base.Util.GetAllTypesOf(tKey.Key)/*.Where(_ => _ != typeof(Entity<Guid>))*/)
                {
                    try
                    {
                        modelBuilder.Entity(type)
                                    .ToTable(type.Name)
                                    .Property("Id").HasColumnName("Id")
                                    .HasColumnType(tKey.Value)
                                    .HasDefaultValue()
                                    ;
                    }
                    catch { }
                }
            }
        }
    }
}