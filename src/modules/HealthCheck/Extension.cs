using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ws.Core.Extensions.HealthCheck
{
    public class Extension : Base.Extension
    {
        private Options _options => GetOptions<Options>() ?? new Options();
        public override void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            base.Execute(serviceCollection, serviceProvider);
            var builder = serviceCollection.AddHealthChecks();
            
            // checks
            var checks = _options.Checks;
            if (checks != null)
            {
                // storage
                if (checks.Storage != null && checks.Storage.Any())
                    foreach(var storage in checks.Storage)
                        builder.AddDiskStorageHealthCheck(_ => _.AddDrive(storage.Driver,storage.MinimumFreeMb), $"storage-{storage.Name}",storage.Status);

                // tcp
                if (checks.Tcp != null && checks.Tcp.Any())
                    foreach (var tcp in checks.Tcp) 
                        builder.AddTcpHealthCheck(_ => _.AddHost(tcp.Host,tcp.Port),$"tcp-{tcp.Name}",tcp.Status);
                
                // http
                if (checks.Http != null && checks.Http.Any())
                    foreach (var http in checks.Http)
                        builder.AddUrlGroup(new Uri(http.Url), $"http-{http.Name}", http.Status);
            }

        }
        public override void Execute(IApplicationBuilder applicationBuilder, IServiceProvider serviceProvider)
        {
            base.Execute(applicationBuilder, serviceProvider);
            
            applicationBuilder.UseHealthChecks(_options.Route, new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

        }
    }
}