using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace core.Extensions.Cache
{
    class Extension : Base.Extension
    {
        public override void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            base.Execute(serviceCollection, serviceProvider);
            serviceCollection.AddMemoryCache();
            serviceCollection.AddTransient(typeof(ICachedRepository<>), typeof(Repository.InMemory<>));
        }
    }
}