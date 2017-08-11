using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace core.Extensions.Message
{
    public class Extension: Base.Extension
    {
        public override void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            base.Execute(serviceCollection, serviceProvider);
            serviceCollection.AddTransient<IMessage, EmailMessage>();
        }        
    }
}
