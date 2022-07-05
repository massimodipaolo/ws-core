using Carter;
using ExtCore.Infrastructure.Actions;
using Hangfire;
using Hangfire.InMemory;

namespace xCore.Extensions;

public class Hangfire : Ws.Core.Extensions.Base.Extension /*ExtCore.Infrastructure.ExtensionBase*/, IConfigureBuilder, IConfigureApp, ICarterModule
{
    private (string prefix, string tag) _route => GetApiRoute(Name);
    public override string Name => typeof(Hangfire).Name;

    public override int Priority => 500;

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet($"{_route.prefix}/{{text}}".ToLower(), enqueue).WithTags(_route.tag);
    }

    private void enqueue(string text) => BackgroundJob.Enqueue(() => Console.WriteLine($"🄵🄾🄾 - {text} - 𝖇𝖆𝖗"));

    public override void Execute(WebApplicationBuilder builder, IServiceProvider? serviceProvider = null)
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

    public override void Execute(WebApplication app)
    {
        app.UseHangfireDashboard("/hangfire", options: new DashboardOptions() { StatsPollingInterval = 10 /*seconds*/ * 1000, DisplayStorageConnectionString = true });
        enqueue($"{nameof(Extensions)}/{Name} started!");
    }
}
