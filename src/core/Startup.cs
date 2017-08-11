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
        protected IConfigurationRoot _config;
        protected ILoggerFactory _logger { get; set; }
        protected DateTime _uptime = DateTime.Now;

        public Startup(IHostingEnvironment hostingEnvironment, ILoggerFactory loggerFactory)
        {
            _env = hostingEnvironment;
            _logger = loggerFactory;
            if (_env.IsDevelopment())
                _logger.AddConsole();

            var builder = new ConfigurationBuilder()
              .SetBasePath(_env.ContentRootPath)
              .AddJsonFile("app-settings.json", optional: true) 
              .AddJsonFile($"app-settings.{_env.EnvironmentName}.json", optional: true)              
              .AddJsonFile("ext-settings.json", optional: true, reloadOnChange: true) //IOptionsSnapshot to live reload              
              .AddJsonFile($"ext-settings.{_env.EnvironmentName}.json", optional: true, reloadOnChange: true)
              .AddEnvironmentVariables(); //override any config files / user secrets          
            
            _config = builder.Build();
        }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions(); //.Configure<Configuration.Settings>(_config);

            services.AddSingleton<IConfiguration>(_config);            

            services.AddExtCore();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IServiceProvider services)
        {            
            //Error handling
            if (_env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
                        
            app.UseExtCore();

            //Data                         
            /*
                try
                {
                    using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                    {
                        var _db = serviceScope.ServiceProvider.GetService<Data.AppDbContext>();
                        _db.Database.EnsureCreated();
                        //Migrate()
                        //Seed
                    }
                }
                catch { }
            */
            //Autentication: https://stormpath.com/blog/authentication-asp-net-core

            //Mvc              
            //app.UseMvc();


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
                     $"Extensions: {string.Join(" | ", ExtensionManager.GetInstances<IExtension>().Select(ext => ext.Name))}\n";

                 await context.Response.WriteAsync(msg);
             }));


        }

    }
}
