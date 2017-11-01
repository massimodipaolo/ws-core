using System;
using System.Collections.Generic;
using core.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace core.Extensions.Api
{
    public class Extension : Base.Extension
    {
        public override void Execute(IServiceCollection services, IServiceProvider serviceProvider)
        {
            base.Execute(services, serviceProvider);
            services.AddMvc();
        }

        public override void Execute(IApplicationBuilder applicationBuilder, IServiceProvider serviceProvider)
        {
            base.Execute(applicationBuilder, serviceProvider);
            applicationBuilder.UseMvcWithDefaultRoute();
        }
    }
}
