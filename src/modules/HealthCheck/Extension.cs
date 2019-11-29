using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
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
                    foreach (var storage in checks.Storage.Where(_ => !string.IsNullOrEmpty(_.Driver)))
                        builder.AddDiskStorageHealthCheck(_ => _.AddDrive(storage.Driver, storage.MinimumFreeMb), $"storage-{storage.Name}", storage.Status);

                // tcp
                if (checks.Tcp != null && checks.Tcp.Any())
                    foreach (var tcp in checks.Tcp.Where(_ => !string.IsNullOrEmpty(_.Host)))
                        builder.AddTcpHealthCheck(_ => _.AddHost(tcp.Host, tcp.Port), $"tcp-{tcp.Name}", tcp.Status);

                // http
                if (checks.Http != null && checks.Http.Any())
                    foreach (var http in checks.Http.Where(_ => !string.IsNullOrEmpty(_.Url)))
                        builder.AddUrlGroup(new Uri(http.Url), $"http-{http.Name}", http.Status);
            }

        }
        public override void Execute(IApplicationBuilder applicationBuilder, IServiceProvider serviceProvider)
        {
            base.Execute(applicationBuilder, serviceProvider);

            if (_options.Routes == null || !_options.Routes.Any())
                _options.Routes = new List<Options.Route> {
                        new Options.Route { Path = "/healthz", ContentType = Options.RouteContentType.text, SkipChecks = true },
                        new Options.Route { Path = "/healthz/checks", ContentType = Options.RouteContentType.json, SkipChecks = false }
                    };

            foreach (var route in _options.Routes)
            {
                var opt = new HealthCheckOptions
                {
                    Predicate = _ => !route.SkipChecks,
                    AllowCachingResponses = false,
                    ResultStatusCodes = {
                                [HealthStatus.Healthy] = 200,
                                [HealthStatus.Degraded] = 200,
                                [HealthStatus.Unhealthy] = 503
                             }
                };
                if (route.ContentType == Options.RouteContentType.json)
                    opt.ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse;

                applicationBuilder.UseHealthChecks(route.Path, opt);
            }


        }
    }
}