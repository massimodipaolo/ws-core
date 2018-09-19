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

            EF.Options options = new EF.Extension()._options;

            // Ignore common unsupported array of primitive types
            modelBuilder
                .Ignore<int[]>()
                .Ignore<long[]>()
                .Ignore<Guid[]>()
                .Ignore<string[]>();

            // Ignore custom type
            foreach (var type in options.Ignore)
            {
                Type t = Type.GetType(type);
                if (t != null)
                    modelBuilder.Ignore(t);
            }

            // Mappings
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
                        EF.Options.MappingConfig opt = options.Mappings.FirstOrDefault(_ => _.Name == type.Name);

                        var entityBuilder = modelBuilder.Entity(type)
                                    .ToTable(opt?.Table ?? type.Name, opt?.Schema ?? "dbo");

                        entityBuilder.Property("Id").HasColumnName(opt?.IdColumnName ?? "Id")
                                    .IsUnicode(false)
                                    .HasMaxLength(tKey.Value)
                                    //.HasColumnType(tKey.Value)
                                    .HasDefaultValue();

                        if (opt?.Properties != null)
                            foreach (var p in opt.Properties)
                            {
                                if (p.Ignore)
                                    entityBuilder.Ignore(p.Name);
                                else
                                    entityBuilder.Property(p.Name).HasColumnName(string.IsNullOrEmpty(p.Column) ? p.Name : p.Column);
                            }


                    }
                    catch { }
                }
            }
        }
    }
}