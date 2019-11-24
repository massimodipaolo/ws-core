using ExtCore.Infrastructure.Actions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Hangfire;
using Hangfire.Annotations;
using Hangfire.Common;
using Hangfire.Dashboard;
using Hangfire.States;
using Hangfire.Storage;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace web.Extensions
{
    public class HangfireExtension : ExtCore.Infrastructure.ExtensionBase, IConfigureAction, IConfigureServicesAction
    {
        public override string Name => typeof(HangfireExtension).Name;

        public virtual int Priority => 250;

        //IConfigureServicesAction
        public virtual void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            var _config = serviceProvider.GetRequiredService<IConfiguration>();
            if (true /*_config.GetSection($"{Startup._appConfigSectionRoot}:cronJob")?.GetValue<bool>("Enabled") ?? false*/)
            {
                serviceCollection.AddHangfire(x => x.UseSqlServerStorage(
                    _config[$"{Ws.Core.Extensions.Base.Configuration.SectionRoot}:assemblies:Ws.Core.Extensions.Data.EF.SqlServer:options:connections:0:connectionString"],
                    new Hangfire.SqlServer.SqlServerStorageOptions() { QueuePollInterval = TimeSpan.FromMinutes(1) })
                    );
                serviceCollection.AddHealthChecks().AddHangfire(_ =>
                {
                    _.MinimumAvailableServers = 1;
                    _.MaximumJobsFailed = 2;
                }, failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded);
            }
             
        }

        //IConfigureAction
        public virtual void Execute(IApplicationBuilder applicationBuilder, IServiceProvider serviceProvider)
        {
            var _appConfigMonitor = applicationBuilder.ApplicationServices?.GetRequiredService<IOptions<AppConfig>>();
            if (true /*_appConfigMonitor?.Value?.CronJob?.Enabled ?? false*/)
            { 

                applicationBuilder.UseHangfireServer(new BackgroundJobServerOptions
                {
                    HeartbeatInterval = new System.TimeSpan(0, 1, 0),
                    ServerCheckInterval = new System.TimeSpan(0, 1, 0),
                    SchedulePollingInterval = new System.TimeSpan(0, 1, 0)
                    //,Queues = new string[] { "important", "default" }
                });

                applicationBuilder.UseHangfireDashboard(
                    "/hangfire",
                    new DashboardOptions() { Authorization = new[] { new HangfireDashboardAuthorizationFilter() }, StatsPollingInterval = 10 /*seconds*/ * 1000, DisplayStorageConnectionString = true }
                );

                //Code.Job.Schedule();
            }
        }

        private class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
        {
            public bool Authorize([NotNull] DashboardContext context)
            {
                // pre-auth in AuthorizationExtension
                return true;
            }
        }
    }
}
