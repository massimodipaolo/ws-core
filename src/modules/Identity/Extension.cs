using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Ws.Core.Extensions.Identity
{
    public class Extension: Base.Extension
    {
        private Options options => GetOptions<Options>();

        public override void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            base.Execute(serviceCollection, serviceProvider);

            /*
            serviceCollection.Configure<IISOptions>(iis =>
            {                
                iis.AuthenticationDisplayName = "Windows";
                iis.AutomaticAuthentication = false;
            });
            */

            var builder = serviceCollection.AddIdentityServer(opt =>
            {
                opt.Events.RaiseSuccessEvents = true;
                opt.Events.RaiseFailureEvents = true;
                opt.Events.RaiseErrorEvents = true;
                opt.Events.RaiseInformationEvents = true;                
            });
            if (options.InMemory != null && options.InMemory.Enable)
            {
                if (options.InMemory.IdentityResources != null)
                    builder
                        .AddInMemoryIdentityResources(options.InMemory.IdentityResources);

                if (options.InMemory.ApiResources != null)
                    builder
                        .AddInMemoryApiResources(options.InMemory.ApiResources);

                if (options.InMemory.Clients != null)
                    builder                        
                        .AddInMemoryClients(options.InMemory.Clients);                    

                if (options.InMemory.PersistedGrants)
                    builder.AddInMemoryPersistedGrants();

                if (options.InMemory.Caching)
                    builder.AddInMemoryCaching();
            }

            if (options.DeveloperSigningCredential)
                builder.AddDeveloperSigningCredential();

            if (options.JwtBearerClientAuthentication)
                builder
                    .AddJwtBearerClientAuthentication();                    

            if (options.TestUsers != null && options.TestUsers.Any())
                builder.AddTestUsers(options.TestUsers.ToList());            
        }

        public override void Execute(IApplicationBuilder applicationBuilder, IServiceProvider serviceProvider)
        {
            base.Execute(applicationBuilder, serviceProvider);

            applicationBuilder.UseIdentityServer();
            
        }        
    }
}
