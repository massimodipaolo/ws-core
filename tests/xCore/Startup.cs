using Carter;
using Microsoft.Extensions.Options;

namespace xCore;

public class Startup : Ws.Core.Startup<Ws.Core.AppConfig>
{
    public Startup(WebApplicationBuilder builder) : base(builder.Environment, (IConfiguration)builder.Services.BuildServiceProvider().GetRequiredService<IConfiguration>()) { }

    public void Add(WebApplicationBuilder builder) => ConfigureServices(builder);

    public override void ConfigureServices(WebApplicationBuilder builder)
    {
        base.ConfigureServices(builder);
        Ws.Core.AppInfo<Ws.Core.AppConfig>.Set(env: env, config: config, services: builder.Services);

        builder.Services.AddCarter();
    }

    public void Use(WebApplication app)
    {
        var services = app.Services;

        app.MapGet("/api/log", (Ws.Core.Extensions.Data.IRepository<Log, int> repo) => repo.List.FirstOrDefault());
        app.MapGet("/api/log/{id}", (int id, Ws.Core.Extensions.Data.IRepository<Log, int> repo) => repo.Find(id));

        Configure(
            app,
            services.GetRequiredService<IOptionsMonitor<Ws.Core.AppConfig>>(),
            services.GetRequiredService<IOptionsMonitor<Ws.Core.Extensions.Base.Configuration>>(),
            app.Lifetime,
            services.GetRequiredService<ILogger<Ws.Core.Program>>()
            );

        app.MapCarter();
    }
    public override void Configure(WebApplication app, IOptionsMonitor<Ws.Core.AppConfig> appConfigMonitor, IOptionsMonitor<Ws.Core.Extensions.Base.Configuration> extConfigMonitor, IHostApplicationLifetime lifetime, ILogger<Ws.Core.Program> logger)
    {
        logger.LogInformation("Start");

        Ws.Core.AppInfo<Ws.Core.AppConfig>.Set(app: app, appConfigMonitor: appConfigMonitor, extConfigMonitor: extConfigMonitor, loggerFactory: app.Services?.GetRequiredService<ILoggerFactory>(), lifetime: lifetime);

        base.Configure(app, appConfigMonitor, extConfigMonitor, lifetime, logger);
        
        app.MapGet("/ping", () => @"
██████╗░░█████╗░███╗░░██╗░██████╗░
██╔══██╗██╔══██╗████╗░██║██╔════╝░
██████╔╝██║░░██║██╔██╗██║██║░░██╗░
██╔═══╝░██║░░██║██║╚████║██║░░╚██╗
██║░░░░░╚█████╔╝██║░╚███║╚██████╔╝
╚═╝░░░░░░╚════╝░╚═╝░░╚══╝░╚═════╝░");

        //shutdown
        lifetime.ApplicationStopping.Register(() =>
        {
            logger.LogInformation("Shutdown");
        }
        );
    }

}
