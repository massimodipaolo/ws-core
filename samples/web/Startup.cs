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

namespace web
{
    public class Startup : Ws.Core.Startup<AppConfig>
    {
        public Startup(IWebHostEnvironment hostingEnvironment, IConfiguration configuration, ILoggerFactory loggerFactory) : base(hostingEnvironment, configuration, loggerFactory)
        {
            _logger.CreateLogger<Startup>().LogInformation("Start");
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddResponseCompression(_ => _.EnableForHttps = true);

            base.ConfigureServices(services);

            services.AddTransient(typeof(Ws.Core.Extensions.Data.AppDbContext), typeof(Code.AppDbContextExt));

            /* override repository */
            // Cms            
            //services.AddTransient(typeof(core.Extensions.Data.IRepository<Server.Models.Page, int>), typeof(core.Extensions.Data.Repository.SqlRepository<Server.Models.Page, int>));

            Ws.Core.AppInfo<AppConfig>.Set(env: _env, config: _config, loggerFactory: _logger, services: services);
        }

        public override void Configure(IApplicationBuilder app, IOptionsMonitor<AppConfig> appConfigMonitor, IOptionsMonitor<Ws.Core.Extensions.Base.Configuration> extConfigMonitor, IHostApplicationLifetime applicationLifetime)
        {
            app.UseResponseCompression();

            Ws.Core.AppInfo<AppConfig>.Set(app: app, appConfigMonitor: appConfigMonitor, extConfigMonitor: extConfigMonitor, applicationLifetime: applicationLifetime);

            base.Configure(app, appConfigMonitor, extConfigMonitor, applicationLifetime);

            //await Code.AppInfo.Init();

            //shutdown
            applicationLifetime.ApplicationStopping.Register(() =>
            {
                _logger.CreateLogger<Startup>().LogInformation("Shutdown");
            }
            );
        }

    }
}
