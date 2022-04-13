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
        public Startup(WebApplicationBuilder builder): base(builder.Environment, (IConfiguration)builder.Services.BuildServiceProvider().GetRequiredService<IConfiguration>()) 
        { }

        public void Add(WebApplicationBuilder builder) => ConfigureServices(builder);

        public override void ConfigureServices(WebApplicationBuilder builder)
        {
            //builder.Services.AddResponseCompression(_ => _.EnableForHttps = true);
            //builder.Services.AddControllers();
            //builder.Services.AddTransient<Ws.Core.Extensions.HealthCheck.Checks.AppLog.IAppLogService, Code.HealthCheckAppLogService>();

            base.ConfigureServices(builder);

            //builder.Services.AddTransient(typeof(Ws.Core.Extensions.Data.AppDbContext), typeof(Code.AppDbContextExt));

            /* override repository */
            // Cms            
            //services.AddTransient(typeof(core.Extensions.Data.IRepository<Server.Models.Page, int>), typeof(core.Extensions.Data.Repository.SqlRepository<Server.Models.Page, int>));
            /*
            services.AddTransient(
                typeof(Ws.Core.Extensions.Data.IRepository<Code.User, Guid>), 
                typeof(Ws.Core.Extensions.Data.Repository.EF.SqlServer<Code.User, Guid>)
                );
            */

            Ws.Core.AppInfo<Ws.Core.AppConfig>.Set(env: env, config: config, services: builder.Services);
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
            IHostApplicationLifetime applicationLifetime, 
            ILogger<Ws.Core.Program> logger
            )
        {
            logger.LogInformation("Start");

            //app.UseResponseCompression();

            Ws.Core.AppInfo<Ws.Core.AppConfig>.Set(app: app, appConfigMonitor: appConfigMonitor, extConfigMonitor: extConfigMonitor, loggerFactory: app.Services?.GetRequiredService<ILoggerFactory>(), lifetime: applicationLifetime);

            base.Configure(app, appConfigMonitor, extConfigMonitor, applicationLifetime, logger);

            app.MapGet("/ping", () => "pong");

            //shutdown
            applicationLifetime.ApplicationStopping.Register(() =>
            {
                logger.LogInformation("Shutdown");
            }
            );
        }

    }
}
