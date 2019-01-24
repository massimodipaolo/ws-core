using System;
using System.Collections.Generic;
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

            var builder = serviceCollection.AddIdentityServer();
            if (_options.InMemory != null)
            {
                builder
                    .AddInMemoryIdentityResources(_options.InMemory.IdentityResources)
                    .AddInMemoryApiResources(_options.InMemory.ApiResources)
                    .AddInMemoryClients(_options.InMemory.Clients);

                if (_options.InMemory.PersistedGrants)
                    builder.AddInMemoryPersistedGrants();

                if (_options.InMemory.Caching)
                    builder.AddInMemoryCaching();
            }
        }

        public override void Execute(IApplicationBuilder applicationBuilder, IServiceProvider serviceProvider)
        {
            base.Execute(applicationBuilder, serviceProvider);

            applicationBuilder.UseIdentityServer();
        }        
    }
}
