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

        public override void Execute(WebApplicationBuilder builder, IServiceProvider? serviceProvider = null)
        {
            base.Execute(builder, serviceProvider);

            builder.Services
                        .AddMemoryCache()
                        .AddDistributedMemoryCache();

            // cache client
            Type implementation;
            Type genericsImplementation;

            // service
            switch (_type)
            {   
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
        }
    }
}