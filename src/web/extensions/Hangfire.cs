using ExtCore.Infrastructure.Actions;
using Hangfire;
using Hangfire.Dashboard;
using Microsoft.Extensions.Options;
using static Ws.Core.Extensions.Base.Extension;

namespace ws.bom.oven.web.extensions;
public class Hangfire : Ws.Core.Extensions.Base.Extension, IConfigureBuilder, IConfigureApp
{
    private static bool _addInit { get; set; } = false;
    private static readonly Ws.Core.Extensions.Base.Util.Locker _addMutexInit = new();
    public override string Name => typeof(Hangfire).Name;
    public override int Priority => 500;

    public class DashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        private readonly WebApplication? _app;
        public DashboardAuthorizationFilter(WebApplication app)
        {
            _app = app;
        }
        public bool Authorize(DashboardContext context)
        {
            return
                _app?.Environment?.IsProduction() == false
                ||
                context.GetHttpContext()?.User?.Identity?.IsAuthenticated == true;
        }
    }

    public override void Add(WebApplicationBuilder builder, IServiceProvider? serviceProvider = null)
    {
        //var _config = builder..Build().Services.GetService<IOptions<code.AppConfig>>();
        if (/*(_config?.Value?.CronJob?.Enabled ?? false) &&*/!_addInit)
            using (_addMutexInit.Lock())
                if (!_addInit)
                {
                    builder.Services
                        .AddHangfire(_ => _.UseInMemoryStorage())
                        .AddHangfireServer(_ =>
                        {
                            _.HeartbeatInterval = new System.TimeSpan(0, 1, 0);
                            _.ServerCheckInterval = new System.TimeSpan(0, 1, 0);
                            _.SchedulePollingInterval = new System.TimeSpan(0, 1, 0);
                            _.Queues = new string[] { "default" };
                        });
                    builder.Services.AddHealthChecks()
                        .AddHangfire(_ =>
                        {
                            _.MinimumAvailableServers = 1;
                            _.MaximumJobsFailed = 2;
                        }, tags: new[] { "tool", "cron" }, failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded);
                    //Code.Job.Schedule(_appConfigMonitor?.Value?.CronJob?.RecurringJobsCronExpression);
                }
    }
    public override void Use(WebApplication app)
    {
        app.UseHangfireDashboard("/hangfire", options: new DashboardOptions()
        {
            Authorization = new[] { new DashboardAuthorizationFilter(app) },
            StatsPollingInterval = 10 /*seconds*/ * 1000,
            DisplayStorageConnectionString = true
        });
    }
}

public class Job
{
    private static readonly Dictionary<string, System.Linq.Expressions.Expression<Func<Task>>> _recurringJobs =
        new() {
                /* cms */
                {$"{nameof(services.PayloadCms)}.{services.PayloadCms.Sync}", () => services.PayloadCms.Sync()},
        };

    public static void Schedule(Dictionary<string, string> recurringJobsCron)
    {
        /* -- Hanfire syntax --            
         1) Fire and forget 
        //BackgroundJob.Enqueue(() => Console.WriteLine("Hello, world!"));            

         2) Calling methods with delay
        //BackgroundJob.Schedule(() => Console.WriteLine("Hello, world"),TimeSpan.FromDays(1));

         3) Performing recurrent tasks
        //RecurringJob.AddOrUpdate(() => Console.Write("Easy!"), Cron.Daily); //CRON expression: http://en.wikipedia.org/wiki/Cron#CRON_expression

         4) Remove recurring job
         //RecurringJob.RemoveIfExists("some-id");

         5) Trigger recurring job
         //RecurringJob.Trigger("some-id");            
         */

        /* Recurring job */
        foreach (var _job in _recurringJobs)
            RecurringJob.AddOrUpdate(
                _job.Key,
                _job.Value,
                recurringJobsCron.ContainsKey(_job.Key) && !string.IsNullOrEmpty(recurringJobsCron[_job.Key]) ? recurringJobsCron[_job.Key] : Cron.Never(),
                TimeZoneInfo.Local);
    }
}