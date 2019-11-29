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

                // memory
                if (checks.Memory != null)
                    builder.AddProcessAllocatedMemoryHealthCheck(checks.Memory.MaximumAllocatedMb, $"memory-allocated", checks.Memory.Status);

                //services
                if (checks.WinService != null && checks.WinService.Any())
                    foreach (var service in checks.WinService.Where(_ => !string.IsNullOrEmpty(_.ServiceName)))
                        builder.AddWindowsServiceHealthCheck(service.ServiceName, _ => _.Status == System.ServiceProcess.ServiceControllerStatus.Running, $"service-{service.Name}", service.Status);

                // process
                if (checks.Process != null && checks.Process.Any())
                    foreach (var process in checks.Process.Where(_ => !string.IsNullOrEmpty(_.ProcessName)))
                        builder.AddProcessHealthCheck(process.ProcessName, _ => _.Any(p => p.HasExited == false), $"process-{process.Name}", process.Status);

                // tcp
                if (checks.Tcp != null && checks.Tcp.Any())
                    foreach (var tcp in checks.Tcp.Where(_ => !string.IsNullOrEmpty(_.Host)))
                        builder.AddTcpHealthCheck(_ => _.AddHost(tcp.Host, tcp.Port), $"tcp-{tcp.Name}", tcp.Status);

                // http
                if (checks.Http != null && checks.Http.Any())
                    foreach (var http in checks.Http.Where(_ => !string.IsNullOrEmpty(_.Url)))
                        builder.AddUrlGroup(new Uri(http.Url), $"http-{http.Name}", http.Status);
            }

            //ui
            if (_options.Ui?.Enabled == true)
            {
                serviceCollection.AddHealthChecksUI(_options.Ui.DbPath, _ =>
                {
                    if (_options.Ui.Endpoints != null && _options.Ui.Endpoints.Any())
                        foreach (var endpoint in _options.Ui.Endpoints.Where(_ => !string.IsNullOrEmpty(_.Uri)))
                            _.AddHealthCheckEndpoint(endpoint.Name, endpoint.Uri);

                    if (_options.Ui.Webhooks != null && _options.Ui.Webhooks.Any())
                        foreach (var hook in _options.Ui.Webhooks.Where(_ => !string.IsNullOrEmpty(_.Uri)))
                            try
                            {
                                _.AddWebhookNotification(
                                    hook.Name,
                                    hook.Uri,
                                    hook.Payload,
                                    hook.RestorePayload
                                    );
                            }
                            catch { }

                    _.SetEvaluationTimeInSeconds(_options.Ui.EvaluationTimeinSeconds);
                    _.SetMinimumSecondsBetweenFailureNotifications(_options.Ui.MinimumSecondsBetweenFailureNotifications);
                });
            }

        }
        public override void Execute(IApplicationBuilder applicationBuilder, IServiceProvider serviceProvider)
        {
            base.Execute(applicationBuilder, serviceProvider);

            applicationBuilder.UseEndpoints(config =>
             {
                 foreach (var route in 
                 _options.Routes
                 ??
                 new List<Options.Route> {
                        new Options.Route { Path = "/healthz", ContentType = Options.RouteContentType.text, SkipChecks = true },
                        new Options.Route { Path = "/healthz/checks", ContentType = Options.RouteContentType.json, SkipChecks = false }
                    }
                 )
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

                     var builder = config.MapHealthChecks(route.Path, opt);

                     // policy
                     if (route.AuthPolicies != null && route.AuthPolicies.Any())
                         builder.RequireAuthorization(route.AuthPolicies.ToArray());

                     // hosts
                     if (route.AuthHosts != null && route.AuthHosts.Any())
                         builder.RequireHost(route.AuthHosts.ToArray());
                 }

                 //ui
                 var ui = _options.Ui;
                 if (ui != null && ui.Enabled)
                 {
                     var ui_builder = config.MapHealthChecksUI(_ =>
                     {
                         _.UIPath = _options.Ui.Route;
                         _.ApiPath = _options.Ui.RouteApi;
                         _.WebhookPath = _options.Ui.RouteWebhook;
                         if (!string.IsNullOrEmpty(_options.Ui.InjectCss))
                             try
                             {
                                 _.AddCustomStylesheet(_options.Ui.InjectCss);
                             }
                             catch { }
                     });

                     // policy
                     if (_options.Ui.AuthPolicies != null && _options.Ui.AuthPolicies.Any())
                         ui_builder.RequireAuthorization(_options.Ui.AuthPolicies.ToArray());

                     // hosts
                     if (_options.Ui.AuthHosts != null && _options.Ui.AuthHosts.Any())
                         ui_builder.RequireHost(_options.Ui.AuthHosts.ToArray());
                 }

             });
        }
    }
}