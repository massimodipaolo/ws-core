using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Ws.Core.Extensions.Data.Cache;

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
        builder.Services.AddDistributedMemoryCache();

        //DI
        builder.Services.AddSingleton<IExpirationTier<MemoryCache>>(_ => new ExpirationTier<MemoryCache>(options.EntryExpirationInMinutes));
        builder.Services.TryAddSingleton(typeof(ICache), typeof(MemoryCache));
        builder.Services.TryAddSingleton(typeof(ICache<>), typeof(MemoryCache<>));
    }
}