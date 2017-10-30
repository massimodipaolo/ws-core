using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace core.Extensions.Identity
{
        public class Extension: Base.Extension
    {
        public override void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            base.Execute(serviceCollection, serviceProvider);
            serviceCollection.AddIdentityServer()                
                                                .AddInMemoryClients(new IdentityServer4.Models.Client[] { new IdentityServer4.Models.Client() })
                                                .AddInMemoryIdentityResources(new IdentityServer4.Models.IdentityResource[] { new IdentityServer4.Models.IdentityResource() });
        }

        public override void Execute(IApplicationBuilder applicationBuilder, IServiceProvider serviceProvider)
        {
            base.Execute(applicationBuilder, serviceProvider);
            applicationBuilder.UseIdentityServer();
        }
        
    }
}
