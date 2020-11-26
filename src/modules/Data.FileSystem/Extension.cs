using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Ws.Core.Extensions.Data.FileSystem
{
    public class Extension : Base.Extension
    {
        internal Options Options => GetOptions<Options>();
        public override void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            base.Execute(serviceCollection, serviceProvider);
            serviceCollection.TryAddTransient(typeof(IRepository<,>), typeof(Repository.FileSystem<,>));
        }
    }
}