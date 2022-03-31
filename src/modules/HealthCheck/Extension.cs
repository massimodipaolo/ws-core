using HealthChecks.UI.Client;
using HealthChecks.UI.Core;
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
        private Options options => GetOptions<Options>() ?? new Options();
        public override void Execute(WebApplicationBuilder builder, IServiceProvider serviceProvider = null)
        {
            base.Execute(builder, serviceProvider);
            var hcBuilder = builder.Services.AddHealthChecks();

            // checks
            var checks = options.Checks;
            if (checks != null)
            {
                // storage
                if (checks.Storage != null && checks.Storage.Any())
                    foreach (var storage in checks.Storage.Where(_ => !string.IsNullOrEmpty(_.Driver)))
                        hcBuilder.AddDiskStorageHealthCheck(_ => _.AddDrive(storage.Driver, storage.MinimumFreeMb), $"storage-{storage.Name}", storage.Status, storage.Tags);

                // memory
                if (checks.Memory != null)
                    hcBuilder.AddProcessAllocatedMemoryHealthCheck(checks.Memory.MaximumAllocatedMb, $"memory-allocated", checks.Memory.Status, checks.Memory.Tags);

                // win services
#pragma warning disable CA1416
                if (checks.WinService != null && checks.WinService.Any() && OperatingSystem.IsWindows())
                    foreach (var service in checks.WinService.Where(_ => !string.IsNullOrEmpty(_.ServiceName)))
                        hcBuilder.AddWindowsServiceHealthCheck(service.ServiceName, _ => _.Status == System.ServiceProcess.ServiceControllerStatus.Running, name: $"service-{service.Name}", failureStatus: service.Status, tags: service.Tags);
#pragma warning restore CA1416

                // process
                if (checks.Process != null && checks.Process.Any())
                    foreach (var process in checks.Process.Where(_ => !string.IsNullOrEmpty(_.ProcessName)))
                        hcBuilder.AddProcessHealthCheck(process.ProcessName, _ => _.Any(p => p.HasExited == false), $"process-{process.Name}", process.Status, process.Tags);

                // tcp
                if (checks.Tcp != null && checks.Tcp.Any())
                    foreach (var tcp in checks.Tcp.Where(_ => !string.IsNullOrEmpty(_.Host)))
                        hcBuilder.AddTcpHealthCheck(_ => _.AddHost(tcp.Host, tcp.Port), $"tcp-{tcp.Name}", tcp.Status, tcp.Tags);

                // http
                if (checks.Http != null && checks.Http.Any())
                    foreach (var http in checks.Http.Where(_ => !string.IsNullOrEmpty(_.Url)))
                        hcBuilder.AddUrlGroup(new Uri(http.Url), $"http-{http.Name}", http.Status, http.Tags);

                // app log
                if (checks.AppLog != null)
                    hcBuilder.AddAppLog(checks.AppLog, tags: checks.AppLog.Tags);
            }

            //ui
            if (options.Ui?.Enabled == true)
            {
                builder.Services.AddHealthChecksUI(_ =>
                {
                    if (options.Ui.Endpoints != null && options.Ui.Endpoints.Any())
                        foreach (var endpoint in options.Ui.Endpoints.Where(_ => !string.IsNullOrEmpty(_.Uri)))
                            _.AddHealthCheckEndpoint(endpoint.Name, endpoint.Uri);

                    if (options.Ui.Webhooks != null && options.Ui.Webhooks.Any())
                        foreach (var hook in options.Ui.Webhooks.Where(_ => !string.IsNullOrEmpty(_.Uri)))
                            try
                            {
                                _.AddWebhookNotification(
                                    hook.Name,
                                    hook.Uri,
                                    hook.Payload,
                                    hook.RestorePayload,
                                    //shouldNotifyFunc: report => DateTime.UtcNow.Hour >= 8 && DateTime.UtcNow.Hour <= 23,
                                    customMessageFunc: (report) =>
                                    {
                                        var failing = report.Entries.Where(e => e.Value.Status == UIHealthStatus.Unhealthy);
                                        return $"{failing.Count()} healthchecks are failing: {string.Join("/", failing.Select(_ => _.Key))} {Environment.NewLine + "--------------" + Environment.NewLine}";
                                    },
                                    customDescriptionFunc: report =>
                                    {
                                        var failing = report.Entries.Where(e => e.Value.Status == UIHealthStatus.Unhealthy);
                                        return @$"{string.Join(Environment.NewLine + "--------------" + Environment.NewLine,
                                            failing.Select(f => $"{f.Key}: {f.Value.Description} {Environment.NewLine} " +
                                            $"{Newtonsoft.Json.JsonConvert.SerializeObject(f.Value.Data, Newtonsoft.Json.Formatting.Indented)}"))}";
                                    }
                                    );
                            }
                            catch { }

                    _.SetEvaluationTimeInSeconds(options.Ui.EvaluationTimeinSeconds);
                    _.SetMinimumSecondsBetweenFailureNotifications(options.Ui.MinimumSecondsBetweenFailureNotifications);
                })
                .AddInMemoryStorage();
            }

        }
        public override void Execute(WebApplication app)
        {
            base.Execute(app);

            foreach (var route in options.Routes ??
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

                var builder = app.MapHealthChecks(route.Path, opt);

                // policy
                if (route.AuthPolicies != null && route.AuthPolicies.Any())
                    builder.RequireAuthorization(route.AuthPolicies.ToArray());

                // hosts
                if (route.AuthHosts != null && route.AuthHosts.Any())
                    builder.RequireHost(route.AuthHosts.ToArray());
            }

            //ui
            var ui = options.Ui;
            if (ui != null && ui.Enabled)
            {
                var ui_builder = app.MapHealthChecksUI(_ =>
                {
                    _.UIPath = options.Ui.Route;
                    _.ApiPath = options.Ui.RouteApi;
                    _.WebhookPath = options.Ui.RouteWebhook;
                    if (!string.IsNullOrEmpty(options.Ui.InjectCss))
                        try
                        {
                            _.AddCustomStylesheet(options.Ui.InjectCss);
                        }
                        catch { }
                });

                // policy
                if (options.Ui.AuthPolicies != null && options.Ui.AuthPolicies.Any())
                    ui_builder.RequireAuthorization(options.Ui.AuthPolicies.ToArray());

                // hosts
                if (options.Ui.AuthHosts != null && options.Ui.AuthHosts.Any())
                    ui_builder.RequireHost(options.Ui.AuthHosts.ToArray());
            }
        }
    }
}