using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Builder;

namespace Ws.Core.Extensions.Data.Cache.SqlServer
{
    public class Extension : Base.Extension
    {
        private Options options => GetOptions<Options>();

        public override void Execute(WebApplicationBuilder builder, IServiceProvider serviceProvider = null)
        {
            base.Execute(builder, serviceProvider);

            var connectionString = options.Client?.ConnectionString ?? "Server=.;Database=Cache;Trusted_Connection=True;";
            var schema = options.Client?.SchemaName ?? "dbo";
            var table = options.Client?.TableName ?? "Entry";

            builder.Services
                .AddHealthChecks()
                .AddSqlServer(
                    connectionString,
                    healthQuery: $"select top 1 Id from [{schema}].[{table}] with(nolock)",
                    name: "cache-sqlserver",
                    failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded,
                    tags: new[] { "cache", "db", "sql", "sqlserver" }
                    );

            builder.Services
                .AddDistributedSqlServerCache(_ =>
                { _.ConnectionString = connectionString; _.SchemaName = schema; _.TableName = table; }
                );

            //DI
            builder.Services.AddSingleton<IExpirationTier<SqlCache>>(_ => new ExpirationTier<SqlCache>(options.EntryExpirationInMinutes));
            builder.Services.TryAddSingleton(typeof(ICache), typeof(SqlCache));
            builder.Services.TryAddSingleton(typeof(ICache<>), typeof(SqlCache<>));
        }
    }
}