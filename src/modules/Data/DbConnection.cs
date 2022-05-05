using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Ws.Core.Extensions.Data
{
    public interface IDbConnectionFunctionWrapper
    {
        Func<Type, DbConnection> Func { get; }
    }

    public class DbConnection
    {
        public string Name { get; set; } = "default";
        public string ConnectionString { get; set; }
        public string Database { get; set; }
        public ServiceResolverCriteria ServiceResolver { get; set; } = new ServiceResolverCriteria();
        public class ServiceResolverCriteria
        {
            public ServiceResolverSelector Include { get; set; } = new ServiceResolverSelector();
            public ServiceResolverSelector Exclude { get; set; } = new ServiceResolverSelector();
        }
        public class ServiceResolverSelector
        {
            public string[] Assembly { get; set; } = Array.Empty<string>();
            public string[] Namespace { get; set; } = Array.Empty<string>();
            public string[] FullName { get; set; } = Array.Empty<string>();
        }
    }

    public class DbConnectionSelector
    {
        public IEnumerable<Type> Entities;
        public DbConnection DbConnection;
        public DbConnectionSelector(DbConnection dbConnection)
        {
            DbConnection = dbConnection;
        }

        public static Hashtable Collection(
            WebApplicationBuilder builder,
            HashSet<DbConnectionSelector> connectionSelectorTable
            )
        {
            // repository resolver
            var provider = builder.Services.BuildServiceProvider().CreateScope()?.ServiceProvider;
            var entities = Ws.Core.Extensions.Base.Util.GetAllTypesOf(typeof(IEntity));

            // normalize resolver rules
            var serviceResolver = connectionSelectorTable.Select(_ => _.DbConnection.ServiceResolver);
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

            foreach (var row in connectionSelectorTable)
            {
                var resolver = row.DbConnection.ServiceResolver;
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

            Hashtable connectionSelectorHashtable = new();
            foreach (var entity in entities)
                connectionSelectorHashtable.Add(entity.FullName, connectionSelectorTable.FirstOrDefault(_ => _.Entities.Contains(entity))?.DbConnection);

            return connectionSelectorHashtable;
        }

    }
}
