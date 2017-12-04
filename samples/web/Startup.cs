using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System;

namespace web
{
    public class Startup : core.Startup 
    {
        public Startup(IHostingEnvironment hostingEnvironment, IConfiguration configuration, ILoggerFactory loggerFactory) : base(hostingEnvironment, configuration, loggerFactory)
        {
            
        }

        public override void ConfigureServices(IServiceCollection services) 
        {
            base.ConfigureServices(services);
        }

        public override void Configure(IApplicationBuilder app,IOptionsMonitor<core.Extensions.Base.Configuration> extMonitor, IApplicationLifetime applicationLifetime) {
            base.Configure(app,extMonitor,applicationLifetime);            
        }
    }
}
