using Microsoft.Extensions.DependencyInjection;
using System;

namespace core.Extensions.Data.Cache
{
    public class Extension : Base.Extension
    {
        public override void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            base.Execute(serviceCollection, serviceProvider);
            serviceCollection.AddMemoryCache();            
            serviceCollection.AddTransient(typeof(ICachedRepository<>), typeof(Repository.InMemory<>));
        }
    }
}