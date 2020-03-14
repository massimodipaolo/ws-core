using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Ws.Core.Extensions.Base;

namespace web
{
    public class Startup : Ws.Core.Startup<AppConfig>
    {
        public static string _appConfigSectionRoot;
        public Startup(IWebHostEnvironment hostingEnvironment, IConfiguration configuration) : base(hostingEnvironment, configuration)
        {
            _appConfigSectionRoot = appConfigSectionRoot;
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddResponseCompression(_ => _.EnableForHttps = true);

            base.ConfigureServices(services);

            services.AddTransient(typeof(Ws.Core.Extensions.Data.AppDbContext), typeof(Code.AppDbContextExt));

            /* override repository */
            // Cms            
            //services.AddTransient(typeof(core.Extensions.Data.IRepository<Server.Models.Page, int>), typeof(core.Extensions.Data.Repository.SqlRepository<Server.Models.Page, int>));
            /*
            services.AddTransient(
                typeof(Ws.Core.Extensions.Data.IRepository<Code.User, Guid>), 
                typeof(Ws.Core.Extensions.Data.Repository.EF.SqlServer<Code.User, Guid>)
                );
            */

            Ws.Core.AppInfo<AppConfig>.Set(env: _env, config: _config, services: services);
        }

        public override void Configure(IApplicationBuilder app, IOptionsMonitor<AppConfig> appConfigMonitor, IOptionsMonitor<Ws.Core.Extensions.Base.Configuration> extConfigMonitor, IHostApplicationLifetime applicationLifetime, ILogger<Ws.Core.Program> logger)
        {
            logger.LogInformation("Start");

            app.UseResponseCompression();

            Ws.Core.AppInfo<AppConfig>.Set(app: app, appConfigMonitor: appConfigMonitor, extConfigMonitor: extConfigMonitor, loggerFactory: app.ApplicationServices?.GetRequiredService<ILoggerFactory>(), applicationLifetime: applicationLifetime);

            base.Configure(app, appConfigMonitor, extConfigMonitor, applicationLifetime, logger);

            //await Code.AppInfo.Init();

            //shutdown
            applicationLifetime.ApplicationStopping.Register(() =>
            {
                logger.LogInformation("Shutdown");
            }
            );
        }

    }
}
