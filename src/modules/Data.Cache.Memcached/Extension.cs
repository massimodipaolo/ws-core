using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Ws.Core.Extensions.Data.Cache.Memcached
{
    public class Extension : Base.Extension
    {
        private Options options => GetOptions<Options>();

        public override void Execute(WebApplicationBuilder builder, IServiceProvider serviceProvider = null)
        {
            base.Execute(builder, serviceProvider);

            if (options.Client != null)
            {
                // default entry expiration
                if (Options.EntryExpirationInMinutes == null)
                    Options.EntryExpirationInMinutes = new Core.Extensions.Data.Cache.Options.Duration();

                // init/override default cache profile
                //CacheEntryOptions.Expiration.Set();

                // service                
                builder.Services.AddEnyimMemcached(config.GetSection($"{ConfigSectionPathOptions}:Client"));

                //DI
                builder.Services.TryAddSingleton(typeof(ICache), typeof(MemcachedCache));
                builder.Services.TryAddSingleton(typeof(ICache<>), typeof(MemcachedCache<>));
                builder.Services.TryAddTransient(typeof(ICacheRepository<,>), typeof(Repository.CachedRepository<,>));
            }
        }

        public override void Execute(WebApplication app)
        {
            if (options.Client != null)
                app.UseEnyimMemcached();
        }
    }
}
