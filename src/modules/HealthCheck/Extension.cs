using HealthChecks.UI.Client;
using HealthChecks.UI.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace Ws.Core.Extensions.HealthCheck;

public class Extension : Base.Extension
{
    private Options options => GetOptions<Options>() ?? new Options();
    public override void Add(WebApplicationBuilder builder, IServiceProvider? serviceProvider = null)
    {
        base.Add(builder, serviceProvider);
        _add(builder);
    }

    private void _add(WebApplicationBuilder builder)
    {
        var hcBuilder = builder.Services.AddHealthChecks();

        // checks
        var checks = options.Checks;
        if (checks != null)
            _addChecks(hcBuilder, checks);

        //ui
        if (options.Ui?.Enable == true)
            _addUi(builder, options.Ui);
    }

    public override void Use(WebApplication app)
    {
        base.Use(app);
        _map(app, options);
    }

    #region Add
    private static void _addChecks(IHealthChecksBuilder hcBuilder, Options.CheckEntries checks)
    {
        _checkStorage(hcBuilder, checks);
        _checkMemory(hcBuilder, checks);
        if (OperatingSystem.IsWindows())
            _checkWinService(hcBuilder, checks);
        _checkProcess(hcBuilder, checks);
        _checkTcp(hcBuilder, checks);
        _checkHttp(hcBuilder, checks);
        _checkAppLog(hcBuilder, checks);
    }
    private static void _checkStorage(IHealthChecksBuilder hcBuilder, Options.CheckEntries checks)
    {
        if (checks.Storage != null && checks.Storage.Any())
            foreach (var storage in checks.Storage.Where(_ => !string.IsNullOrEmpty(_.Driver)))
#pragma warning disable CS8604 // Already checked
                hcBuilder.AddDiskStorageHealthCheck(_ => _.AddDrive(storage.Driver, storage.MinimumFreeMb), $"storage-{storage.Name}", storage.Status, storage.Tags);
#pragma warning restore CS8604 // Possible null reference argument.
    }

    private static void _checkMemory(IHealthChecksBuilder hcBuilder, Options.CheckEntries checks)
    {
        if (checks.Memory != null)
            hcBuilder.AddProcessAllocatedMemoryHealthCheck(checks.Memory.MaximumAllocatedMb, $"memory-allocated", checks.Memory.Status, checks.Memory.Tags);
    }

    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    private static void _checkWinService(IHealthChecksBuilder hcBuilder, Options.CheckEntries checks)
    {
        if (checks.WinService != null && checks.WinService.Any() && OperatingSystem.IsWindows())
            foreach (var service in checks.WinService.Where(_ => !string.IsNullOrEmpty(_.ServiceName)))
#pragma warning disable CS8604 // Possible null reference argument.
                hcBuilder.AddWindowsServiceHealthCheck(service.ServiceName, _ => _.Status == System.ServiceProcess.ServiceControllerStatus.Running, name: $"service-{service.Name}", failureStatus: service.Status, tags: service.Tags);
#pragma warning restore CS8604 // Possible null reference argument.
    }

    private static void _checkProcess(IHealthChecksBuilder hcBuilder, Options.CheckEntries checks)
    {
        if (checks.Process != null && checks.Process.Any())
            foreach (var process in checks.Process.Where(_ => !string.IsNullOrEmpty(_.ProcessName)))
#pragma warning disable CS8604 // Already checked
                hcBuilder.AddProcessHealthCheck(process.ProcessName, _ => _.Any(p => !p.HasExited), $"process-{process.Name}", process.Status, process.Tags);
#pragma warning restore CS8604 // Possible null reference argument.
    }

    private static void _checkTcp(IHealthChecksBuilder hcBuilder, Options.CheckEntries checks)
    {
        if (checks.Tcp != null && checks.Tcp.Any())
            foreach (var tcp in checks.Tcp.Where(_ => !string.IsNullOrEmpty(_.Host)))
#pragma warning disable CS8604 // Already checked
                hcBuilder.AddTcpHealthCheck(_ => _.AddHost(tcp.Host, tcp.Port), $"tcp-{tcp.Name}", tcp.Status, tcp.Tags);
#pragma warning restore CS8604 // Possible null reference argument.
    }

    private static void _checkHttp(IHealthChecksBuilder hcBuilder, Options.CheckEntries checks)
    {
        if (checks.Http != null && checks.Http.Any())
            foreach (var http in checks.Http.Where(_ => !string.IsNullOrEmpty(_.Url)))
#pragma warning disable CS8604 // Already checked
                hcBuilder.AddUrlGroup(new Uri(http.Url), $"http-{http.Name}", http.Status, http.Tags);
#pragma warning restore CS8604 // Possible null reference argument.
    }

    private static void _checkAppLog(IHealthChecksBuilder hcBuilder, Options.CheckEntries checks)
    {
        if (checks.AppLog != null)
            hcBuilder.AddAppLog(checks.AppLog, tags: checks.AppLog.Tags);
    }

    private static void _addUi(WebApplicationBuilder builder, Options.UiOptions options)
    {
        builder.Services.AddHealthChecksUI(settings =>
        {
            _uiEndpoints(settings, options);
            _uiWebhooks(settings, options);
            settings.SetEvaluationTimeInSeconds(options.EvaluationTimeinSeconds);
            settings.SetMinimumSecondsBetweenFailureNotifications(options.MinimumSecondsBetweenFailureNotifications);
        })
        .AddInMemoryStorage();
    }

    private static void _uiEndpoints(HealthChecks.UI.Configuration.Settings settings, Options.UiOptions options)
    {
        if (options.Endpoints != null && options.Endpoints.Any())
            foreach (var endpoint in options.Endpoints.Where(_ => !string.IsNullOrEmpty(_.Name) && !string.IsNullOrEmpty(_.Uri)))
#pragma warning disable CS8604 //Already checked
                settings.AddHealthCheckEndpoint(endpoint.Name, _uiEndpointUri(endpoint.Uri));
#pragma warning restore CS8604 // Possible null reference argument.
    }

    /// <summary>
    /// https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks/issues/410#issuecomment-733106956
    /// </summary>
    /// <param name="uri"></param>
    /// <returns></returns>
    private static string _uiEndpointUri(string uri)
    {
        if (Uri.IsWellFormedUriString(uri, UriKind.Relative))
        {
            var envUrls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS");
            if (envUrls != null)
            {
                var uris = envUrls
                    .Split(';')
                    .Select(url => Regex.Replace(url, @"^(?<scheme>https?):\/\/((\+)|(\*)|(0.0.0.0))(?=[\:\/]|$)", "${scheme}://localhost"))
                    .Select(uri => new Uri(uri, UriKind.Absolute))
                    .ToArray();
                foreach (var scheme in new string[] { "http", "https" })
                {
                    var httpEndpoint = uris.FirstOrDefault(uri => uri.Scheme == scheme);
                    if (httpEndpoint != null)
                        return new UriBuilder(httpEndpoint.Scheme, httpEndpoint.Host, httpEndpoint.Port, uri).ToString();
                }
            }
        }
        return uri;
    }

    private static void _uiWebhooks(HealthChecks.UI.Configuration.Settings settings, Options.UiOptions options)
    {
        if (options.Webhooks != null && options.Webhooks.Any())
            foreach (var hook in options.Webhooks.Where(_ => !string.IsNullOrEmpty(_.Uri)))
                try
                {
                    settings.AddWebhookNotification(
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
                catch (Exception ex) { logger?.LogError(ex, ""); }
    }
    #endregion

    #region Map
    private static void _map(WebApplication app, Options options)
    {
        _mapRoutes(app, options);
        _mapUi(app, options);
    }

    /// <summary>
    /// Map hc routes
    /// </summary>
    /// <param name="app"></param>
    /// <param name="options"></param>
    private static void _mapRoutes(WebApplication app, Options options)
    {
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

            if (!string.IsNullOrEmpty(route.Path))
            {
                var builder = app.MapHealthChecks(route.Path, opt);

                // policy
                if (route.AuthPolicies != null && route.AuthPolicies.Any())
                    builder.RequireAuthorization(route.AuthPolicies.ToArray());

                // hosts
                if (route.AuthHosts != null && route.AuthHosts.Any())
                    builder.RequireHost(route.AuthHosts.ToArray());
            }
        }
    }

    /// <summary>
    /// Map hc ui
    /// </summary>
    /// <param name="app"></param>
    /// <param name="options"></param>
    private static void _mapUi(WebApplication app, Options options)
    {
        var ui = options.Ui;
        if (ui != null && ui.Enable)
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
                    catch (Exception ex) { logger?.LogError(ex, ""); }
            });

            // policy
            if (options.Ui.AuthPolicies != null && options.Ui.AuthPolicies.Any())
                ui_builder.RequireAuthorization(options.Ui.AuthPolicies.ToArray());

            // hosts
            if (options.Ui.AuthHosts != null && options.Ui.AuthHosts.Any())
                ui_builder.RequireHost(options.Ui.AuthHosts.ToArray());
        }
    }
    #endregion
}