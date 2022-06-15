using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace web
{
    public class Startup: Ws.Core.Startup<Ws.Core.AppConfig>
    {
        public Startup(WebApplicationBuilder builder): base(builder.Environment, builder.Services.BuildServiceProvider().GetRequiredService<IConfiguration>()) 
        { }

        public void Add(WebApplicationBuilder builder) => ConfigureServices(builder);

        public override void ConfigureServices(WebApplicationBuilder builder)
        {
            base.ConfigureServices(builder);
            Ws.Core.AppInfo<Ws.Core.AppConfig>.ConfigureServices(env: env, config: config, services: builder.Services);
        }

        public void Use(WebApplication app)
        {
            var services = app.Services;
            Configure(
                app,
                services.GetRequiredService<IOptionsMonitor<Ws.Core.AppConfig>>(),
                services.GetRequiredService<IOptionsMonitor<Ws.Core.Extensions.Base.Configuration>>(),
                app.Lifetime,
                services.GetRequiredService<ILogger<Ws.Core.Program>>()
                );
        }
        public override void Configure(
            WebApplication app, 
            IOptionsMonitor<Ws.Core.AppConfig> appConfigMonitor, 
            IOptionsMonitor<Ws.Core.Extensions.Base.Configuration> extConfigMonitor, 
            IHostApplicationLifetime lifetime, 
            ILogger<Ws.Core.Program> logger
            )
        {
            logger.LogInformation("Start");

            Ws.Core.AppInfo<Ws.Core.AppConfig>.ConfigureApp(app: app, appConfigMonitor: appConfigMonitor, extConfigMonitor: extConfigMonitor, loggerFactory: app.Services?.GetRequiredService<ILoggerFactory>(), lifetime: lifetime);

            base.Configure(app, appConfigMonitor, extConfigMonitor, lifetime, logger);

            app.MapGet("/ping", () => "pong");

            //shutdown
            lifetime.ApplicationStopping.Register(() =>
            {
                logger.LogInformation("Shutdown");
            }
            );
        }

    }
}
