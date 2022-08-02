using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Ws.Core.Extensions.Data.Cache.Redis;

public class Extension : Base.Extension
{
    private Options options => GetOptions<Options>();
    public override void Add(WebApplicationBuilder builder, IServiceProvider? serviceProvider = null)
    {
        base.Add(builder, serviceProvider);
        _add(builder);
    }

    private void _add(WebApplicationBuilder builder)
    {
        var host = options?.Client?.Configuration ?? "localhost:6379";
        builder.Services
            .AddHealthChecks()
            .AddRedis(host, name: "cache-redis", failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded, tags: new[] { "cache", "redis" });

        if (options?.Client != null)
        {
            builder.Services
            .AddDistributedRedisCache(_ =>
            {
                _.Configuration = host;
                _.InstanceName = options?.Client?.InstanceName ?? "master";
            }
            );

            //DI
            builder.Services.AddSingleton<IExpirationTier<RedisCache>>(_ => new ExpirationTier<RedisCache>(options.EntryExpirationInMinutes));
            builder.Services.TryAddSingleton(typeof(ICache), typeof(RedisCache));
            builder.Services.TryAddSingleton(typeof(ICache<>), typeof(RedisCache<>));
        }
    }
}