using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace core.Extensions.Data.FileSystem
{
    public class Extension : Base.Extension
    {
        public override void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            base.Execute(serviceCollection, serviceProvider);
            serviceCollection.TryAddTransient(typeof(IRepository<>), typeof(Repository.FileSystem<>));
        }
    }
}