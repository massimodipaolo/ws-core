using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace core.Extensions.Data.Cache
{
    public class Extension : Base.Extension
    {
        private Base.Options.DataCacheOptions _options => GetOptions<Base.Options.DataCacheOptions>();
        private Base.Options.DataCacheOptions.Types _type => _options?.Type ?? Base.Options.DataCacheOptions.Types.Memory;

        public override void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            base.Execute(serviceCollection, serviceProvider);

            // cache client
            Type clientType = typeof(DistributedCache);

            // repository 
            Type repositoryType = typeof(Repository.CachedRepository<>);

            // service
            switch (_type)
            {
                case Base.Options.DataCacheOptions.Types.Distributed:                    
                    serviceCollection.AddDistributedMemoryCache();                    
                    break;
                case Base.Options.DataCacheOptions.Types.Redis:                    
                    serviceCollection.AddDistributedRedisCache(_ => { _.Configuration = _options.RedisOptions.Configuration; _.InstanceName = _options.RedisOptions.InstanceName; });
                    break;
                case Base.Options.DataCacheOptions.Types.SqlServer:
                    serviceCollection.AddDistributedSqlServerCache(_ => { _.ConnectionString = ""; });
                    break;
                default:                    
                    serviceCollection.AddMemoryCache();
                    clientType = typeof(MemoryCache);                    
                    break;
            }
            
            serviceCollection.AddTransient(typeof(ICache), clientType);
            serviceCollection.AddTransient(typeof(ICacheRepository<>), repositoryType);
            
        }
    }    
}