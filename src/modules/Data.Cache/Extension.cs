using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Builder;

namespace Ws.Core.Extensions.Data.Cache
{
    public class Extension : Base.Extension
    {
        private Options options => GetOptions<Options>();
        private Options.Types _type => options?.Type ?? Options.Types.Memory;

        public override void Execute(WebApplicationBuilder builder, IServiceProvider serviceProvider = null)
        {
            base.Execute(builder, serviceProvider);

            // init/override default cache profile
            // CacheEntryOptions.Expiration.Set();

            builder.Services
                        .AddMemoryCache()
                        .AddDistributedMemoryCache();

            // cache client
            Type implementation;
            Type genericsImplementation;

            // service
            switch (_type)
            {   
                /*
                case Options.Types.Redis:
                    {
                        var host = _options.RedisOptions?.Configuration ?? "localhost:6379";
                        serviceCollection.AddHealthChecks().AddRedis(host,failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded);
                        serviceCollection.AddDistributedRedisCache(_ => { _.Configuration = host; _.InstanceName = _options.RedisOptions?.InstanceName ?? "master"; });
                    }
                    break;
                case Options.Types.SqlServer:
                    serviceCollection.AddDistributedSqlServerCache(_ => { _.ConnectionString = _options.SqlOptions?.ConnectionString ?? "Server=.;Database=Cache;Trusted_Connection=True;"; _.SchemaName = _options.SqlOptions?.SchemaName ?? "dbo"; _.TableName = _options.SqlOptions?.TableName ?? "Entry"; });
                    break;
                    */
                case Options.Types.Distributed:
                    implementation = typeof(DistributedCache);
                    genericsImplementation = typeof(DistributedCache<>);
                    break;
                default:                                        
                    implementation = typeof(MemoryCache);
                    genericsImplementation = typeof(MemoryCache<>);
                    break;
            }

            //DI
            builder.Services.TryAddSingleton(typeof(ICache), implementation);
            builder.Services.TryAddSingleton(typeof(ICache<>), genericsImplementation);
            builder.Services.TryAddTransient(typeof(ICacheRepository<,>), typeof(Repository.CachedRepository<,>));
            /*
            serviceCollection.TryAddTransient(typeof(IEntityChangeEvent<,>), typeof(Repository.EntityChangeHandler<,>));
            serviceCollection.TryAddTransient(typeof(IEntityChangeEvent<>), typeof(Repository.EntityChangeHandler<>));
            serviceCollection.TryAddTransient(typeof(IEntityChangeEvent), typeof(Repository.EntityChangeHandler));
            */
        }
    }
}