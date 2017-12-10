using System;
using System.Collections.Generic;
using System.Linq;
using ExtCore.Infrastructure;
using ExtCore.WebApplication.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace core
{
    public class Startup
    {
        protected IHostingEnvironment _env { get; set; }
        protected IConfiguration _config;
        private IServiceCollection _services;
        protected ILoggerFactory _logger { get; set; }
        protected DateTime _uptime = DateTime.Now;
        private string _extLastConfigAssembliesSerialized { get; set; }

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
            _services = services;

            _services.AddOptions();

            _services.AddSingleton<IConfiguration>(_config);

            _services.Configure<Extensions.Base.Configuration>(_config.GetSection("Configuration"));

            core.Extensions.Base.Extension.Init(_services, _services.BuildServiceProvider());

            _services.AddExtCore(_config["Configuration:Path"] != null ? $"{_env.ContentRootPath}{System.IO.Path.DirectorySeparatorChar}{_config["Configuration:Path"]}" : null);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public virtual void Configure(IApplicationBuilder app, IOptionsMonitor<Extensions.Base.Configuration> extMonitor, IApplicationLifetime applicationLifetime)
        {
            //Error handling
            if (_env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseExtCore();

            Func<IDictionary<string, Extensions.Base.Configuration.Assembly>, string> _extSerialize = list => string.Join(" | ", list.Where(ext => ext.Value.Priority > 0).OrderBy(ext => ext.Value.Priority).Select(_ => _.Key));

            _extLastConfigAssembliesSerialized = _extSerialize(extMonitor.CurrentValue.Assemblies);

            extMonitor.OnChange(extConfig =>
            {

                var _extCurrentAssembliesSerialized = _extSerialize(extConfig.Assemblies);
                var isUpdatable = _extCurrentAssembliesSerialized == _extLastConfigAssembliesSerialized;

                _logger.CreateLogger("extMonitor").LogWarning($"Config changed {DateTime.Now}; Is updatable: {isUpdatable} ");

                if (isUpdatable)
                    _extLastConfigAssembliesSerialized = _extCurrentAssembliesSerialized;

                if (!isUpdatable && extConfig.EnableShutDownOnChange)
                {
                    applicationLifetime.StopApplication();
                }
                else
                    ExtCore.Events.Event<core.Extensions.Base.IConfigurationChangeEvent, core.Extensions.Base.ConfigurationChangeContext>
                    .Broadcast(new Extensions.Base.ConfigurationChangeContext()
                    { App = app, Lifetime = applicationLifetime, Configuration = extConfig }
                    );
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
                     $"Extensions: {string.Join(" | ", ExtensionManager.GetInstances<core.Extensions.Base.Extension>().OrderBy(ext => ext.Priority).Select(ext => $"{ext.Name} [{ext.Priority}]"))}\n" +
                     //$"Configuration: {string.Join(" | ", _config.AsEnumerable().Select(conf => $"{conf.Key}:{conf.Value}"))}\n" +
                     //$"Services: {string.Join(" | ", _services.Select(srv=> $"{srv.ServiceType.FullName}:{srv.Lifetime}:{srv.ImplementationType?.FullName}"))}\n" +
                     "";

                 await context.Response.WriteAsync(msg);
             }));


        }

    }
}
