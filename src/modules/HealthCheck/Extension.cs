using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Ws.Core.Extensions.HealthCheck
{
    public class Extension : Base.Extension
    {
        private Options _options => GetOptions<Options>();
        public override void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            base.Execute(serviceCollection, serviceProvider);
            var builder = serviceCollection.AddHealthChecks();
            //builder.AddCheck();
        }
        public override void Execute(IApplicationBuilder applicationBuilder, IServiceProvider serviceProvider)
        {
            base.Execute(applicationBuilder, serviceProvider);
            applicationBuilder.UseEndpoints(config =>
             {
                 config.MapHealthChecks("/healtz", new HealthCheckOptions
                 {
                     Predicate = _ => true,
                     ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                 });
             });
        }
    }
}