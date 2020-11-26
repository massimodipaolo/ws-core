using System;
using System.Collections.Generic;
using System.Linq;
using Ws.Core.Extensions.Base;
using ExtCore.Infrastructure;
using ExtCore.WebApplication.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;

namespace Ws.Core
{
    public class Startup<TOptions> where TOptions : class, IAppConfiguration
    {
        protected IWebHostEnvironment env { get; set; }
        protected IConfiguration config;
        private IServiceCollection services;
        public static readonly DateTime Uptime = DateTime.Now;
        private string _extLastConfigAssembliesSerialized { get; set; }
        protected string AppConfigSectionRoot { get; set; } = "appConfig";

        public Startup(IWebHostEnvironment hostingEnvironment, IConfiguration configuration)
        {
            env = hostingEnvironment;
            config = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public virtual void ConfigureServices(IServiceCollection services)
        {
            this.services = services;

            this.services.AddOptions();

            this.services.AddSingleton(config);

            this.services.Configure<Configuration>(config.GetSection(Core.Extensions.Base.Configuration.SectionRoot));

            this.services.Configure<TOptions>(config.GetSection(AppConfigSectionRoot));

            /*
            var _razorConfig = _config.GetSection(appConfigSectionRoot).Get<TOptions>()?.ToExpando()?.FirstOrDefault(_ => _.Key == "RazorEngine").Value as AppConfig.RazorEngineOptions ?? new AppConfig.RazorEngineOptions();
            _services.AddRazorLight(() =>
            {
                var _builder = new RazorLight.RazorLightEngineBuilder()
                    .AddDefaultNamespaces(_razorConfig?.Namespaces?.ToArray() ?? new[] { "System" });
                if (_razorConfig?.DynamicTemplates != null && _razorConfig.DynamicTemplates.Any())
                    _builder.AddDynamicTemplates(_razorConfig.DynamicTemplates);
                if (_razorConfig?.Assemblies != null && _razorConfig.Assemblies.Any())
                {
                    try
                    {
                        _razorConfig.AdditionalMetadataReferences = new HashSet<Microsoft.CodeAnalysis.MetadataReference>(_razorConfig.Assemblies.Select(path => (Microsoft.CodeAnalysis.MetadataReference)Microsoft.CodeAnalysis.MetadataReference.CreateFromFile(path)));
                        _builder.AddMetadataReferences(_razorConfig.AdditionalMetadataReferences.ToArray());
                    }
                    catch { }
                }
                return _builder
                    .UseMemoryCachingProvider()
                    .Build();
            });
            */

            Extensions.Base.Extension.Init(this.services, this.services.BuildServiceProvider());

            this.services.AddExtCore(config[$"{Extensions.Base.Configuration.SectionRoot}:Folder"] != null ? $"{env.ContentRootPath}{System.IO.Path.DirectorySeparatorChar}{config[$"{Extensions.Base.Configuration.SectionRoot}:Folder"]}" : null);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public virtual void Configure(IApplicationBuilder app, IOptionsMonitor<TOptions> appConfigMonitor, IOptionsMonitor<Extensions.Base.Configuration> extConfigMonitor, IHostApplicationLifetime applicationLifetime, ILogger<Ws.Core.Program> logger)
        {

            //Error handling
            if (env.IsDevelopment() || env.EnvironmentName == "Local" || (appConfigMonitor.CurrentValue?.DeveloperExceptionPage ?? false))
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseExtCore();

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
                    applicationLifetime.StopApplication();
                }
                else
                    ExtCore.Events.Event<Extensions.Base.IConfigurationChangeEvent, Extensions.Base.ConfigurationChangeContext>
                    .Broadcast(new Extensions.Base.ConfigurationChangeContext()
                    { App = app, Lifetime = applicationLifetime, Configuration = extConfig }
                    );
            });

        }

    }
}
