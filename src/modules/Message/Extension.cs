using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ws.Core.Extensions.Message
{
    public class Extension: Base.Extension
    {
        private Options options => GetOptions<Options>();
        public override void Execute(WebApplicationBuilder builder, IServiceProvider serviceProvider = null)
        {
            base.Execute(builder, serviceProvider);
            builder.Services.AddHealthChecks().AddCheck<EmailMessage>("message-email", tags: new[] {"message", "smtp", "email" });
            builder.Services.AddSingleton<IMessageConfiguration>(options);
            builder.Services.AddTransient<IMessage, EmailMessage>();
        }        
    }
}
