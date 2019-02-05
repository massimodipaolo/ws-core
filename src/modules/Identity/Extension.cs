using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace core.Extensions.Identity
{
    public class Extension: Base.Extension
    {
        private Options _options => GetOptions<Options>();

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
            if (_options.InMemory != null && _options.InMemory.Enable)
            {
                if (_options.InMemory.IdentityResources != null)
                    builder
                        .AddInMemoryIdentityResources(_options.InMemory.IdentityResources);

                if (_options.InMemory.ApiResources != null)
                    builder
                        .AddInMemoryApiResources(_options.InMemory.ApiResources);

                if (_options.InMemory.Clients != null)
                    builder                        
                        .AddInMemoryClients(_options.InMemory.Clients);                    

                if (_options.InMemory.PersistedGrants)
                    builder.AddInMemoryPersistedGrants();

                if (_options.InMemory.Caching)
                    builder.AddInMemoryCaching();
            }

            if (_options.DeveloperSigningCredential)
                builder.AddDeveloperSigningCredential();

            if (_options.JwtBearerClientAuthentication)
                builder
                    .AddJwtBearerClientAuthentication();                    

            if (_options.TestUsers != null && _options.TestUsers.Any())
                builder.AddTestUsers(_options.TestUsers.ToList());            
        }

        public override void Execute(IApplicationBuilder applicationBuilder, IServiceProvider serviceProvider)
        {
            base.Execute(applicationBuilder, serviceProvider);

            applicationBuilder.UseIdentityServer();
            
        }        
    }
}
