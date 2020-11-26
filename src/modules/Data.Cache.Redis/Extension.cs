using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Ws.Core.Extensions.Data.Cache.Redis
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

            var host = options?.Client?.Configuration ?? "localhost:6379";
            serviceCollection
                .AddHealthChecks()
                .AddRedis(host, name:"cache-redis", failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded);

            serviceCollection
                //.AddMemoryCache()
                //.AddDistributedMemoryCache()
                .AddDistributedRedisCache(_ =>
                { _.Configuration = host; _.InstanceName = options?.Client?.InstanceName ?? "master"; }
                );

            //DI
            serviceCollection.TryAddSingleton(typeof(ICache), typeof(DistributedCache));
            serviceCollection.TryAddSingleton(typeof(ICache<>), typeof(DistributedCache<>));
            serviceCollection.TryAddTransient(typeof(ICacheRepository<,>), typeof(Repository.CachedRepository<,>));

        }
    }
}