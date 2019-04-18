using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Ws.Core.Extensions.Message
{
    public class Extension: Base.Extension
    {
        private Options _options => GetOptions<Options>();
        public override void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            base.Execute(serviceCollection, serviceProvider);
            serviceCollection.AddSingleton<IMessageConfiguration>(_options);
            serviceCollection.AddTransient<IMessage, EmailMessage>();
        }        
    }
}
