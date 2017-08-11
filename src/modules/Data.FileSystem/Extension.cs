using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace core.Extensions.Data.FileSystem
{
    public class Extension : Base.Extension
    {
        public override void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            base.Execute(serviceCollection, serviceProvider);             
            serviceCollection.AddTransient(typeof(IRepository<>), typeof(Repository.FileSystem<>));
        }
    }
}