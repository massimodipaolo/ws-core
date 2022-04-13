using Carter;
using ExtCore.Infrastructure.Actions;
using Hangfire;
using Hangfire.InMemory;

namespace xCore.Extensions;

public class Hangfire : ExtCore.Infrastructure.ExtensionBase, IConfigureBuilder, IConfigureApp, ICarterModule
{
    public override string Name => typeof(Hangfire).Name;

    public virtual int Priority => 500;

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet($"{nameof(Extensions)}/{Name}/{{text}}".ToLower(), enqueue);
    }

    private void enqueue(string text) => BackgroundJob.Enqueue(() => Console.WriteLine($"🄵🄾🄾 - {text} - 𝖇𝖆𝖗"));

    public void Execute(WebApplicationBuilder builder, IServiceProvider serviceProvider = null)
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

    public void Execute(WebApplication app)
    {
        app.UseHangfireDashboard("/hangfire", options: new DashboardOptions() { StatsPollingInterval = 10 /*seconds*/ * 1000, DisplayStorageConnectionString = true });
        enqueue($"{nameof(Extensions)}/{Name} started!");
    }
}
