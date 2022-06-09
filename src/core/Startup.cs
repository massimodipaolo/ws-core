using Carter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using Ws.Core.Extensions;
using Ws.Core.Extensions.Base;

namespace Ws.Core
{
    public class Startup<TOptions> where TOptions : class, IAppConfiguration
    {
        protected IWebHostEnvironment env { get; set; }
        protected IConfiguration config;
        private IServiceCollection services;
        public static readonly DateTime Uptime = DateTime.Now;
        private string _extLastConfigAssembliesSerialized { get; set; }
        protected string AppConfigSectionRoot { get; set; } = nameof(AppConfig);

        public Startup(IWebHostEnvironment hostingEnvironment, IConfiguration configuration)
        {
            env = hostingEnvironment;
            config = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public virtual void ConfigureServices(WebApplicationBuilder builder)
        {
            services = builder.Services;

            services.AddOptions();

            services.AddSingleton(config);

            services.Configure<Configuration>(config.GetSection(Core.Extensions.Base.Configuration.SectionRoot));

            services.Configure<TOptions>(config.GetSection(AppConfigSectionRoot));

            services.AddSingleton<IAppConfiguration, TOptions>();

            Extensions.Base.Extension.Init(services, services.BuildServiceProvider());

            builder.AddExtCore(config[$"{Extensions.Base.Configuration.SectionRoot}:Folder"] != null ? $"{env.ContentRootPath}{System.IO.Path.DirectorySeparatorChar}{config[$"{Extensions.Base.Configuration.SectionRoot}:Folder"]}" : null, includingSubpaths: true);

            var carterModules = Ws.Core.Extensions.Base.Util.GetAllTypesOf<ICarterModule>();
            if (carterModules.Any())
                builder.Services.AddCarter(configurator: _ => _
                    .WithModules(carterModules.ToArray())
                    );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public virtual void Configure(WebApplication app, IOptionsMonitor<TOptions> appConfigMonitor, IOptionsMonitor<Extensions.Base.Configuration> extConfigMonitor, IHostApplicationLifetime lifetime, ILogger<Ws.Core.Program> logger)
        {

            //Error handling
            if (env.IsDevelopment() || env.EnvironmentName == "Local" || (appConfigMonitor.CurrentValue?.DeveloperExceptionPage ?? false))
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseExtCore();

            app.MapCarter();

            static string _extSerialize(IDictionary<string, Configuration.Assembly> list) => list == null ? null : string.Join(" | ", list?.Where(ext => ext.Value.Priority > 0)?.OrderBy(ext => ext.Value.Priority)?.Select(_ => _.Key));

            _extLastConfigAssembliesSerialized = _extSerialize(extConfigMonitor.CurrentValue.Assemblies);

            extConfigMonitor.OnChange(extConfig =>
            {

                var _extCurrentAssembliesSerialized = _extSerialize(extConfig.Assemblies);
                var isUpdatable = _extCurrentAssembliesSerialized == _extLastConfigAssembliesSerialized;

                logger.LogInformation($"Config changed {DateTime.Now}; Is updatable: {isUpdatable} ");

                if (isUpdatable)
                    _extLastConfigAssembliesSerialized = _extCurrentAssembliesSerialized;

                if (!isUpdatable && extConfig.EnableShutDownOnChange)
                {
                    lifetime.StopApplication();
                }
                else
                    ExtCore.Events.Event<Extensions.Base.IConfigurationChangeEvent, Extensions.Base.ConfigurationChangeContext>
                    .Broadcast(new Extensions.Base.ConfigurationChangeContext()
                    { App = app, Lifetime = lifetime, Configuration = extConfig }
                    );
            });

        }

    }
}
