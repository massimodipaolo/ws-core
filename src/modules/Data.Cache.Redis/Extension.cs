using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Builder;

namespace Ws.Core.Extensions.Data.Cache.Redis
{
    public class Extension : Base.Extension
    {
        private Options options => GetOptions<Options>();

        public override void Execute(WebApplicationBuilder builder, IServiceProvider serviceProvider = null)
        {
            base.Execute(builder, serviceProvider);

            var host = options?.Client?.Configuration ?? "localhost:6379";
            builder.Services
                .AddHealthChecks()
                .AddRedis(host, name:"cache-redis", failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded, tags: new[] { "cache", "redis" });

            builder.Services
                .AddDistributedRedisCache(_ =>
                { 
                    _.Configuration = host; 
                    _.InstanceName = options?.Client?.InstanceName ?? "master";
                }
                );

            //DI
            builder.Services.TryAddSingleton(typeof(ICache), typeof(RedisCache));
            builder.Services.TryAddSingleton(typeof(ICache<>), typeof(RedisCache<>));            
        }
    }
}