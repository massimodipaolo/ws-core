using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Http;

namespace web
{
    public class Startup: core.Startup<AppConfig>
    {
        public Startup(IHostingEnvironment hostingEnvironment, IConfiguration configuration, ILoggerFactory loggerFactory) : base(hostingEnvironment, configuration, loggerFactory)
        {
            
        }

        public override void ConfigureServices(IServiceCollection services) 
        {
            base.ConfigureServices(services);

            services.Configure<AppConfig>(_config.GetSection("appConfig"));
        }

        public override void Configure(IApplicationBuilder app, IOptionsMonitor<AppConfig> appConfigMonitor, IOptionsMonitor<core.Extensions.Base.Configuration> extConfigMonitor, IApplicationLifetime applicationLifetime) {
            base.Configure(app,appConfigMonitor, extConfigMonitor, applicationLifetime);            
        }

    }
}
