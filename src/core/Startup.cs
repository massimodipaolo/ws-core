using ExtCore.Infrastructure;
using ExtCore.WebApplication.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace core
{
    public class Startup
    {
        protected IHostingEnvironment _env { get; set; }
        protected IConfiguration _config;
        protected ILoggerFactory _logger { get; set; }
        protected DateTime _uptime = DateTime.Now;

        public Startup(IHostingEnvironment hostingEnvironment, IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _env = hostingEnvironment;
            _logger = loggerFactory;
            _config = configuration;       
        }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions(); //.Configure<Configuration.Settings>(_config);

            services.AddSingleton<IConfiguration>(_config);            

            services.AddExtCore(_config["Extensions:Path"] != null ? $"{_env.ContentRootPath}{System.IO.Path.DirectorySeparatorChar}{_config["Extensions:Path"]}" : null);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public virtual void Configure(IApplicationBuilder app)
        {            
            //Error handling
            if (_env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
                        
            app.UseExtCore();

            app.Map("/info", _ => _.Run(async (context) =>
             {
                 var msg = @"
..####...#####...#####............####....####...#####...######.
.##..##..##..##..##..##..........##..##..##..##..##..##..##.....
.######..#####...#####...######..##......##..##..#####...####...
.##..##..##......##..............##..##..##..##..##..##..##.....
.##..##..##......##...............####....####...##..##..######.
................................................................
";
                 msg +=
                     "\n" +
                     $"Uptime: {_uptime}\n" +
                     $"ApplicationName: {_env.ApplicationName}\n" +
                     $"Environment: {_env.EnvironmentName}\n" +                     
                     $"MachineName: {Environment.MachineName}\n" +
                     $"ProcessorCount: {Environment.ProcessorCount}\n" +                     
                     $"Extensions: {string.Join(" | ", ExtensionManager.GetInstances<IExtension>().Select(ext => ext.Name))}\n" + 
                     $"Configuration: {string.Join(" | ", _config.AsEnumerable().Select(conf => $"{conf.Key}:{conf.Value}"))}\n" ;

                 await context.Response.WriteAsync(msg);
             }));


        }

    }
}
