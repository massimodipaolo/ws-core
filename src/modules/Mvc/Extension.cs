using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace core.Extensions.Mvc
{
    public class Extension: Base.Extension
    {
        public override IEnumerable<KeyValuePair<int, Action<IServiceCollection>>> ConfigureServicesActionsByPriorities
        {
            get
            {
                var d = new Dictionary<int, Action<IServiceCollection>>();
                d[Priority] = service => service.AddMvc();
                return d;
            }
        }
        public override IEnumerable<KeyValuePair<int, Action<IApplicationBuilder>>> ConfigureActionsByPriorities {
            get
            {
                var d = new Dictionary<int, Action<IApplicationBuilder>>();
                d[Priority] = app => app.UseMvcWithDefaultRoute();
                return d;
            }
        }
    }
}
