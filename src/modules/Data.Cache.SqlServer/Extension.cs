using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Ws.Core.Extensions.Data.Cache.SqlServer
{
    public class Extension : Base.Extension
    {
        private Options options => GetOptions<Options>();

        public override void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            base.Execute(serviceCollection, serviceProvider);

            // default entry expiration
            if (Options.EntryExpirationInMinutes == null)
                Options.EntryExpirationInMinutes = new Cache.Options.Duration();

            var connectionString = options.Client?.ConnectionString ?? "Server=.;Database=Cache;Trusted_Connection=True;";
            var schema = options.Client?.SchemaName ?? "dbo";
            var table = options.Client?.TableName ?? "Entry";

            serviceCollection
                .AddHealthChecks()
                .AddSqlServer(
                    connectionString,
                    healthQuery: $"select top 1 Id from [{schema}].[{table}] with(nolock)",
                    name: "cache-sqlserver",
                    failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded
                    );

            serviceCollection
                //.AddMemoryCache()
                //.AddDistributedMemoryCache()
                .AddDistributedSqlServerCache(_ =>
                { _.ConnectionString = connectionString; _.SchemaName = schema; _.TableName = table; }
                );

            //DI
            serviceCollection.TryAddSingleton(typeof(ICache), typeof(DistributedCache));
            serviceCollection.TryAddSingleton(typeof(ICache<>), typeof(DistributedCache<>));
            serviceCollection.TryAddTransient(typeof(ICacheRepository<,>), typeof(Repository.CachedRepository<,>));

        }
    }
}