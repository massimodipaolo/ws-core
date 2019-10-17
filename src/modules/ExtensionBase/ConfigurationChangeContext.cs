using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ws.Core.Extensions.Base
{
    public class ConfigurationChangeContext
    {
        public IApplicationBuilder App { get; set; }
        public IHostApplicationLifetime Lifetime { get; set; }
        //public IServiceCollection Services { get; set; }
        //public IServiceProvider ServiceProvider { get; set; }
        public Configuration Configuration { get; set; }
    }
}
