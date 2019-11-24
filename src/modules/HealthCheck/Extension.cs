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
                    foreach (var storage in checks.Storage)
                        builder.AddDiskStorageHealthCheck(_ => _.AddDrive(storage.Driver, storage.MinimumFreeMb), $"storage-{storage.Name}", storage.Status);

                // memory
                if (checks.Memory != null)
                    builder.AddProcessAllocatedMemoryHealthCheck(checks.Memory.MaximumAllocatedMb, $"memory-allocated", checks.Memory.Status);

                //services
                if (checks.WinService != null && checks.WinService.Any())
                    foreach (var service in checks.WinService)
                        builder.AddWindowsServiceHealthCheck(service.ServiceName, _ => _.Status == System.ServiceProcess.ServiceControllerStatus.Running, $"service-{service.Name}", service.Status);

                // process
                if (checks.Process != null && checks.Process.Any())
                    foreach (var process in checks.Process)
                        builder.AddProcessHealthCheck(process.ProcessName, _ => _.Any(p => p.HasExited == false), $"process-{process.Name}", process.Status);

                // tcp
                if (checks.Tcp != null && checks.Tcp.Any())
                    foreach (var tcp in checks.Tcp)
                        builder.AddTcpHealthCheck(_ => _.AddHost(tcp.Host, tcp.Port), $"tcp-{tcp.Name}", tcp.Status);

                // http
                if (checks.Http != null && checks.Http.Any())
                    foreach (var http in checks.Http)
                        builder.AddUrlGroup(new Uri(http.Url), $"http-{http.Name}", http.Status);
            }

            //ui
            if (_options.Ui?.Enabled == true)
            {
                serviceCollection.AddHealthChecksUI("healthchecksdb", _ =>
                {
                    if (_options.Ui.Endpoints != null && _options.Ui.Endpoints.Any())
                        foreach (var endpoint in _options.Ui.Endpoints)
                            _.AddHealthCheckEndpoint(endpoint.Name, endpoint.Uri);

                    if (_options.Ui.Webhooks != null && _options.Ui.Webhooks.Any())
                        foreach (var hook in _options.Ui.Webhooks)
                            try
                            {
                                _.AddWebhookNotification(
                                    hook.Name,
                                    hook.Uri,
                                    hook.Payload,
                                    hook.RestorePayload
                                    );
                            }
                            catch {}

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
                 var builder = config.MapHealthChecks(_options?.Route, new HealthCheckOptions
                 {
                     Predicate = _ => true,
                     ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
                     AllowCachingResponses = true,
                     ResultStatusCodes = {
                         [Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy] = 200,
                         [Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded] = 200,
                         [Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy] = 503
                     }
                 });

                 // policy
                 if (_options?.AuthPolicies != null && _options.AuthPolicies.Any())
                     builder.RequireAuthorization(_options.AuthPolicies.ToArray());

                 // hosts
                 if (_options?.AuthHosts != null && _options.AuthHosts.Any())
                     builder.RequireHost(_options.AuthHosts.ToArray());

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