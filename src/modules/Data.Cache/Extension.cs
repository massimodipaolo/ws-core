using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;

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
                    serviceCollection.AddDistributedRedisCache(_ => { _.Configuration = _options.RedisOptions.Configuration; _.InstanceName = _options.RedisOptions.InstanceName; });
                    break;
                case Options.Types.SqlServer:
                    serviceCollection.AddDistributedSqlServerCache(_ => { _.ConnectionString = ""; });
                    break;
                default:                    
                    serviceCollection.AddMemoryCache();
                    clientType = typeof(MemoryCache);                    
                    break;
            }
            
            serviceCollection.AddSingleton(typeof(ICache), clientType);
            serviceCollection.AddTransient(typeof(ICacheRepository<>), repositoryType);           
            
        }
    }    
}