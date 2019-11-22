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
                #warning TODO: storage, memory, process, url
                // storage

                // memory

                // process

                // tcp
                if (checks.Tcp != null && checks.Tcp.Any())
                    checks.Tcp.AsParallel().ForAll(tcp => builder.AddTcpHealthCheck(_ => _.AddHost(tcp.Host,tcp.Port),$"tcp-{tcp.Name}"));

                // url
            }

            //ui
            if (_options.Ui?.Enabled == true)
            {
                serviceCollection.AddHealthChecksUI("healthchecksdb", _ =>
                {
                    foreach (var endpoint in _options.Ui.Endpoints)
                        _.AddHealthCheckEndpoint(endpoint.Name, endpoint.Uri);

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
                        catch(Exception ex) {
                            throw ex;
                        }

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
                     ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
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
                     var ui_builder = config.MapHealthChecksUI(_ => {
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