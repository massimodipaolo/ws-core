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

            var tKeys = new KeyValuePair<Type, int>[] {
                new KeyValuePair<Type,int>(typeof(IEntity<int>),11),
                new KeyValuePair<Type,int>(typeof(IEntity<long>),20),
                new KeyValuePair<Type, int>(typeof(IEntity<Guid>),36),
                new KeyValuePair<Type, int>(typeof(IEntity<string>),255)
            };

            foreach (KeyValuePair<Type, int> tKey in tKeys)
            {
                foreach (Type type in Base.Util.GetAllTypesOf(tKey.Key)/*.Where(_ => _ != typeof(Entity<Guid>))*/)
                {
                    try
                    {
                        modelBuilder.Entity(type)
                                    .ToTable(type.Name)
                                    .Property("Id").HasColumnName("Id")
                                    .IsUnicode(false)
                                    .HasMaxLength(tKey.Value)
                                    //.HasColumnType(tKey.Value)
                                    .HasDefaultValue()
                                    ;
                    }
                    catch { }
                }
            }
        }
    }
}