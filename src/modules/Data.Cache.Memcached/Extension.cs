using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Ws.Core.Extensions.Data.Cache.Memcached;

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
        if (options.Client != null && config != null)
        {
            // service                
            builder.Services.AddEnyimMemcached(config.GetSection($"{ConfigSectionPathOptions}:Client"));

            //DI
            builder.Services.AddSingleton<IExpirationTier<MemcachedCache>>(_ => new ExpirationTier<MemcachedCache>(options.EntryExpirationInMinutes));
            builder.Services.TryAddSingleton(typeof(ICache), typeof(MemcachedCache));
            builder.Services.TryAddSingleton(typeof(ICache<>), typeof(MemcachedCache<>));
        }
    }

    public override void Use(WebApplication app)
    {
        if (options.Client != null)
            app.UseEnyimMemcached();
    }
}
