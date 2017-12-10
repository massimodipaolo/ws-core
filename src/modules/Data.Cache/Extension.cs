using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace core.Extensions.Data.Cache
{
    public class Extension : Base.Extension
    {
        private Options _options => GetOptions<Options>();
        private Options.Types _type => _options?.Type ?? Options.Types.Memory;

        public override void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            base.Execute(serviceCollection, serviceProvider);

            // default entry expiration
            if (Options.EntryExpirationInMinutes == null)
                Options.EntryExpirationInMinutes = new Options.Duration();

            // init/override default cache profile
            //CacheEntryOptions.Expiration.Set();

            // cache client
            Type clientType = typeof(DistributedCache);

            // repository 
            Type repositoryType = typeof(Repository.CachedRepository<>);

            // service
            switch (_type)
            {
                case Options.Types.Distributed:
                    serviceCollection.AddDistributedMemoryCache();
                    break;
                case Options.Types.Redis:
                    serviceCollection.AddDistributedRedisCache(_ => { _.Configuration = _options.RedisOptions?.Configuration ?? "localhost:6379"; _.InstanceName = _options.RedisOptions?.InstanceName ?? "master"; });
                    break;
                case Options.Types.SqlServer:
                    serviceCollection.AddDistributedSqlServerCache(_ => { _.ConnectionString = _options.SqlOptions?.ConnectionString ?? "Server=.;Database=Cache;Trusted_Connection=True;"; _.SchemaName = _options.SqlOptions?.SchemaName ?? "dbo"; _.TableName = _options.SqlOptions?.TableName ?? "Entry"; });
                    break;
                default:
                    serviceCollection.AddMemoryCache();
                    clientType = typeof(MemoryCache);
                    break;
            }

            //DI
            serviceCollection.AddSingleton(typeof(ICache), clientType);
            serviceCollection.TryAddTransient(typeof(ICacheRepository<>), repositoryType);

        }
    }
}