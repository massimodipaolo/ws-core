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

namespace xCore
{
    /// <summary>
    /// Program acts as IClassFixture
    /// </summary>
    public class Program : WebApplicationFactory<Startup>
    {
        protected override IHostBuilder CreateHostBuilder()
        => Ws.Core.Program
        .WebHostBuilder(Array.Empty<string>(), typeof(Program).Assembly, typeof(Startup));

        protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
        {
            // Set the content root path, relative to solution path
            builder.UseSolutionRelativeContentRoot("tests/xCore");
            base.ConfigureWebHost(builder);
        }
    }
}
