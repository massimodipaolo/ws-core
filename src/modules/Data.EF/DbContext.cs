using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Ws.Core.Extensions.Data.EF;

namespace Ws.Core.Extensions.Data.EF
{
    public interface IDbContextFunctionWrapper
    {
        Func<Type, EF.DbContext> Func { get; }
    }
    public class DbContext : Microsoft.EntityFrameworkCore.DbContext 
    {
        static DbContext() { }
        public DbContext(DbContextOptions options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            EF.Options options = new EF.Extension().Options;

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
                        var entityBuilder = modelBuilder.Entity(type);

                        // https://docs.microsoft.com/en-us/ef/core/what-is-new/ef-core-3.0/breaking-changes#totable-on-a-derived-type-throws-an-exception
                        // https://github.com/aspnet/EntityFrameworkCore/issues/11811
                        if (
                            type.BaseType == typeof(object) // i.e. partial class
                            ||
                            type.BaseType == type // IEntity<T>
                            ||
                            type.BaseType?.BaseType == typeof(object) // i.e. Entity<T>
                            )
                            entityBuilder.ToTable(opt?.Table ?? type.Name, opt?.Schema ?? null);

                        // Map Id column
                        var idBuilder = entityBuilder.Property("Id").HasColumnName(opt?.IdColumnName ?? "Id")
                                    .IsUnicode(false)
                                    .HasMaxLength(tKey.Value)
                                    //.HasColumnType(tKey.Value)                                    
                                    ;

                        // https://github.com/aspnet/EntityFrameworkCore/issues/16814
                        if ((opt?.IdHasDefaultValue ?? true) == true)
                            idBuilder.HasDefaultValue();

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

                                if (!string.IsNullOrEmpty(p.HasConversion))
                                {
                                    var _type = p.HasConversion.ToLower();
                                    if (_type == "json")
                                        entityBuilder.Property(p.Name).HasJsonConversion(type.GetProperty(p.Name).PropertyType);
                                    else
                                    {
                                        Type clrType = EF.Options.MappingConfig.PropertyConfig.ColumnClrTypeConversions.FirstOrDefault(_ => _.Name.ToLower() == _type);
                                        if (clrType != null)
                                            entityBuilder.Property(p.Name).HasConversion(clrType);
                                    }
                                }

                            }

                    }
                    catch
                    {
                        throw;
                    }
                }
            }
        }
    }
    
    public class DbContext<TContext> : EF.DbContext where TContext : EF.DbContext
    {
        public DbContext(DbContextOptions<TContext> options) : base(options) { }
    }

    public class DbContextSelector
    {
        public class Item
        {
            public Data.DbConnection.ServiceResolverCriteria ServiceResolver;
            public IEnumerable<Type> Entities;
            public Func<IServiceProvider, EF.DbContext> DbContext;
            public Item(DbConnection.ServiceResolverCriteria serviceResolver, Func<IServiceProvider, EF.DbContext> dbContext)
            {
                ServiceResolver = serviceResolver;
                DbContext = dbContext;
            }
        }

        public static Hashtable Collection(
            WebApplicationBuilder builder,
            HashSet<DbContextSelector.Item> serviceSelectorTable
            )
        {
            // repository resolver
            var provider = builder.Services.BuildServiceProvider().CreateScope()?.ServiceProvider;
            var entities = Ws.Core.Extensions.Base.Util.GetAllTypesOf(typeof(IEntity));

            // normalize resolver rules
            var serviceResolver = serviceSelectorTable.Select(_ => _.ServiceResolver);
            var serviceResolverWithAssemblyInclude = serviceResolver?.Where(_ => _.Include?.Assembly?.Any() == true);
            serviceResolver
                .Except(serviceResolverWithAssemblyInclude).AsParallel()
                .ForAll(_ => _.Exclude.Assembly = _.Exclude.Assembly.Concat(serviceResolverWithAssemblyInclude.SelectMany(_ => _.Include.Assembly).Distinct()).Distinct().ToArray());
            var serviceResolverWithNamespaceInclude = serviceResolver?.Where(_ => _.Include?.Namespace?.Any() == true);
            serviceResolver
                .Except(serviceResolverWithNamespaceInclude).AsParallel()
                .ForAll(_ => _.Exclude.Namespace = _.Exclude.Namespace.Concat(serviceResolverWithNamespaceInclude.SelectMany(_ => _.Include.Namespace).Distinct()).Distinct().ToArray());
            var serviceResolverWithFullNameInclude = serviceResolver?.Where(_ => _.Include?.FullName?.Any() == true);
            serviceResolver
                .Except(serviceResolverWithFullNameInclude).AsParallel()
                .ForAll(_ => _.Exclude.FullName = _.Exclude.FullName.Concat(serviceResolverWithFullNameInclude.SelectMany(_ => _.Include.FullName).Distinct()).Distinct().ToArray());

            foreach (var row in serviceSelectorTable)
            {
                var resolver = row.ServiceResolver;
                row.Entities = entities.Where(_ =>
                (
                (resolver.Include.Assembly.Any() == false || resolver.Include.Assembly?.Any(__ => __ == _.Assembly?.FullName) == true)
                &&
                (resolver.Include.Namespace.Any() == false || resolver.Include.Namespace?.Any(__ => __ == _.Namespace) == true)
                &&
                (resolver.Include.FullName.Any() == false || resolver.Include.FullName?.Any(__ => __ == _.FullName) == true)
                )
                &&
                !(
                resolver.Exclude.Assembly.Any(__ => __ == _.Assembly?.FullName) == true
                ||
                resolver.Exclude.Namespace.Any(__ => __ == _.Namespace) == true
                ||
                resolver.Exclude.FullName.Any(__ => __ == _.FullName) == true
                )
                );
            }

            Hashtable serviceSelectorHashtable = new();
            foreach (var entity in entities)
                try
                {
                    serviceSelectorHashtable.Add(entity.FullName, serviceSelectorTable.FirstOrDefault(_ => _.Entities.Contains(entity))?.DbContext(provider));
                } catch (Exception ex)
                {
                    var _ex = ex;
                }


            return serviceSelectorHashtable;
        }

    }

}