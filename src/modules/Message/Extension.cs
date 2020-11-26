using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ws.Core.Extensions.Message
{
    public class Extension: Base.Extension
    {
        private Options options => GetOptions<Options>();
        public override void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            base.Execute(serviceCollection, serviceProvider);
            serviceCollection.AddHealthChecks().AddCheck<EmailMessage>("message-email");
            serviceCollection.AddSingleton<IMessageConfiguration>(options);
            serviceCollection.AddTransient<IMessage, EmailMessage>();
        }        
    }
}
