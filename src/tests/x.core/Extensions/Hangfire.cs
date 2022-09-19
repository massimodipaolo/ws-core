using Carter;
using ExtCore.Infrastructure.Actions;
using Hangfire;
using Hangfire.Dashboard;

namespace x.core.Extensions;

public class Hangfire : Ws.Core.Extensions.Base.Extension /*ExtCore.Infrastructure.ExtensionBase*/, IConfigureBuilder, IConfigureApp, ICarterModule
{
    private static bool _addInit { get; set; } = false;
    private static readonly Ws.Core.Extensions.Base.Util.Locker _addMutexInit = new();
    private (string prefix, string tag) _route => GetApiRoute(Name);
    public override string Name => typeof(Hangfire).Name;

    public override int Priority => 500;

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet($"{_route.prefix}/{{text}}".ToLower(), enqueue).WithTags(_route.tag);
    }

    private void enqueue(string text) => BackgroundJob.Enqueue(() => Console.WriteLine($"🄵🄾🄾 - {text} - 𝖇𝖆𝖗"));

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
        if (!_addInit)
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
        enqueue($"{nameof(Extensions)}/{Name} started!");
    }
}
