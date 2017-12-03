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
        //protected IOptionsMonitor<IEnumerable<Extensions.Base.Configuration.Assembly>> _extMonitor { get; set; }

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

            services.AddExtCore(_config["Configuration:Path"] != null ? $"{_env.ContentRootPath}{System.IO.Path.DirectorySeparatorChar}{_config["Configuration:Path"]}" : null);

            services.Configure<Extensions.Base.Configuration>(_config.GetSection("Configuration"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public virtual void Configure(IApplicationBuilder app,IOptionsMonitor<Extensions.Base.Configuration> extMonitor)
        {            
            //Error handling
            if (_env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseExtCore();

            /*
            try{
                var optionFactory = new OptionsFactory<Extensions.Base.Configuration>(null, null);
                var optionCache = new OptionsCache<Extensions.Base.Configuration>();
                var monitor = new OptionsMonitor<Extensions.Base.Configuration>(optionFactory, null, optionCache);
                monitor.OnChange(extConfig => {
                    _logger.CreateLogger("extMonitor").LogWarning($"Config changed {DateTime.Now}");
                    ExtCore.Events.Event<core.Extensions.Base.IConfigurationChangeEvent, IApplicationBuilder, core.Extensions.Base.Configuration>.Broadcast(app, extConfig);
                });
            } catch (Exception ex){
                _logger.CreateLogger("Monitor instance").LogError(ex.Message);
            }
            */


            extMonitor.OnChange(extConfig => {
                _logger.CreateLogger("extMonitor").LogWarning($"Config changed {DateTime.Now}");
                ExtCore.Events.Event<core.Extensions.Base.IConfigurationChangeEvent, IApplicationBuilder,core.Extensions.Base.Configuration>.Broadcast(app,extConfig);
            });

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
