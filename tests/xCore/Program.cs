using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ws.Core.Extensions.Data;

namespace xCore
{
    /// <summary>
    /// Program acts as IClassFixture
    /// </summary>
    public class Program<TStartup> : WebApplicationFactory<Startup> where TStartup : class
    {
        protected override IHostBuilder CreateHostBuilder()
        => Ws.Core.Program
        .WebHostBuilder(Array.Empty<string>(), typeof(Program<Startup>).Assembly, typeof(Startup));

        protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
        {            
            builder
                .UseSolutionRelativeContentRoot("tests/xCore") // Set the content root path, relative to solution path
                ;
            base.ConfigureWebHost(builder);
        }
    }
}
