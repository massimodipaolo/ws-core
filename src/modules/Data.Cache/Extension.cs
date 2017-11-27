using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace core.Extensions.Data.Cache
{
    public class Extension : Base.Extension
    {
        private Base.Options.DataCache _options => GetOptions<Base.Options.DataCache>();
        private Base.Options.DataCache.Types _type => _options?.Type ?? Base.Options.DataCache.Types.Memory;

        public override void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            base.Execute(serviceCollection, serviceProvider);            

            //repository 
            Type repositoryType = typeof(Repository.DistributedCache<>);

            // service
            switch (_type)
            {
                case Base.Options.DataCache.Types.Distributed:                    
                    serviceCollection.AddDistributedMemoryCache();                    
                    break;
                case Base.Options.DataCache.Types.Redis:                    
                    serviceCollection.AddDistributedRedisCache(_ => { _.Configuration = _options.RedisOptions.Configuration; _.InstanceName = _options.RedisOptions.InstanceName; });
                    break;
                case Base.Options.DataCache.Types.SqlServer:
                    serviceCollection.AddDistributedSqlServerCache(_ => { _.ConnectionString = ""; });
                    break;
                default:                    
                    serviceCollection.AddMemoryCache();
                    repositoryType = typeof(Repository.MemoryCache<>);
                    break;
            }

            // repository
            serviceCollection.AddTransient(typeof(ICachedRepository<>), repositoryType);

            // cache client
        }
    }    
}