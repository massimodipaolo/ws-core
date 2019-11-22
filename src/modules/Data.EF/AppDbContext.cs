using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Ws.Core.Extensions.Data.EF;

namespace Ws.Core.Extensions.Data
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
            var ignoreTypes = new List<Type>();
            if (options != null && options.Ignore != null && options.Ignore.Length > 0)
                foreach (var type in options.Ignore)
                    try
                    {
                        Type t = Type.GetType(type);
                        if (t != null)
                        {
                            ignoreTypes.Add(t);
                            modelBuilder.Ignore(t);                            
                        }
                    }
                    catch { }

            // JsonConvert: map field with text columun, serializing/deserializing value
            IEnumerable<Type> jsonConvertTypes = null;
            if (options != null && options.JsonConvert != null && options.JsonConvert.Length > 0)
                jsonConvertTypes = options.JsonConvert.Select(_ => Type.GetType(_));

            // Mappings
            var tKeys = new KeyValuePair<Type, int>[] {
                new KeyValuePair<Type,int>(typeof(IEntity<int>),11),
                new KeyValuePair<Type,int>(typeof(IEntity<long>),20),
                new KeyValuePair<Type, int>(typeof(IEntity<Guid>),36),
                new KeyValuePair<Type, int>(typeof(IEntity<string>),255)
            };

            foreach (KeyValuePair<Type, int> tKey in tKeys)
            {
                var types = Base.Util.GetAllTypesOf(tKey.Key)?.Where(t => t != null && !ignoreTypes?.Distinct().Any(i => i == t) == true);                
                foreach (Type type in types)
                {   
                        try
                        {
                            EF.Options.MappingConfig opt = (options?.Mappings ?? new List<EF.Options.MappingConfig>())
                            .FirstOrDefault(_ =>
                                _.Name == type.Name
                                && (string.IsNullOrEmpty(_.NameSpace) || _.NameSpace == type.Namespace)
                            );

                            // Map to db schema,table
                            var entityBuilder = modelBuilder.Entity(type)
                                        .ToTable(opt?.Table ?? type.Name, opt?.Schema ?? "dbo");

                            // Map Id column
                            entityBuilder.Property("Id").HasColumnName(opt?.IdColumnName ?? "Id")
                                        .IsUnicode(false)
                                        .HasMaxLength(tKey.Value)
                                        //.HasColumnType(tKey.Value)
                                        .HasDefaultValue();

                            // Map complex type (or interface) on a text column, serializing/deserializing value
                            if (jsonConvertTypes != null && jsonConvertTypes.Any())
                                foreach (var property in type.GetProperties()
                                    .Where(p => jsonConvertTypes
                                        .Any(jT => jT.IsInterface ? jT.IsAssignableFrom(p.PropertyType) : jT == p.PropertyType)
                                        )
                                    )
                                    if (null == opt?.Properties?.FirstOrDefault(_ => _.Name == property.Name && _.JsonConvert.HasValue && _.JsonConvert.Value == false))
                                        entityBuilder.Property(property.Name).HasJsonConversion(property.PropertyType);

                            // Property based settings
                            if (opt?.Properties != null)
                                foreach (var p in opt.Properties.Where(_ => !string.IsNullOrEmpty(_.Name)))
                                {
                                    // Ignore field
                                    if (p.Ignore)
                                        entityBuilder.Ignore(p.Name);
                                    else
                                        try
                                        {
                                            // Custom map
                                            if (!string.IsNullOrEmpty(p.Column))
                                                entityBuilder.Property(p.Name).HasColumnName(p.Column);
                                        }
                                        catch { }

                                    // Map specific property on a text column, serializing/deserializing value
                                    if (p.JsonConvert.HasValue && p.JsonConvert.Value == true)
                                        entityBuilder.Property(p.Name).HasJsonConversion(type.GetProperty(p.Name).PropertyType);
                                }

                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                }
            }
        }
    }
}