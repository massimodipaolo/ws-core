using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ws.Core.Extensions.Base
{
    public class ConfigurationChangeContext
    {
        public IApplicationBuilder App { get; set; }
        public IApplicationLifetime Lifetime { get; set; }
        //public IServiceCollection Services { get; set; }
        //public IServiceProvider ServiceProvider { get; set; }
        public Configuration Configuration { get; set; }
    }
}
