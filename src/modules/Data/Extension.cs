using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace core.Extensions.Data
{
    class Extension : Base.Extension
    {
        public override void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            base.Execute(serviceCollection, serviceProvider);
            serviceCollection.AddTransient(typeof(IRepository<,>), typeof(Repository.InMemory<,>));
        }
    }
}
