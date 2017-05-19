using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace core.Extensions.Identity
{
        public class Extension: Base.Extension
    {
        
        public override IEnumerable<KeyValuePair<int, Action<IServiceCollection>>> ConfigureServicesActionsByPriorities
        {
            get
            {
                var d = new Dictionary<int, Action<IServiceCollection>>();
                d[Priority] = service => service.AddIdentityServer().AddTemporarySigningCredential();
                return d;
            }
        }

        public override IEnumerable<KeyValuePair<int, Action<IApplicationBuilder>>> ConfigureActionsByPriorities
        {
            get
            {
                var priority = Priority;
                var d = new Dictionary<int, Action<IApplicationBuilder>>();
                d[Priority] = app => app.UseIdentityServer();
                return d;
            }
        }
        
    }
}
