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

        public override void Execute(WebApplicationBuilder builder, IServiceProvider? serviceProvider = null)
        {
            base.Execute(builder, serviceProvider);

            builder.Services
                        .AddDistributedMemoryCache();

            //DI            
            builder.Services.TryAddSingleton(typeof(ICache), typeof(MemoryCache));
            builder.Services.TryAddSingleton(typeof(ICache<>), typeof(MemoryCache<>));
        }
    }
}