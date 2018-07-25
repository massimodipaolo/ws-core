using System;
using System.Collections.Generic;
using System.Linq;
using core.Extensions.Base;
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
    public class Startup<TOptions> where TOptions: IAppConfiguration
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

            _services.Configure<core.Extensions.Base.Configuration>(_config.GetSection(core.Extensions.Base.Configuration.SectionRoot));

            core.Extensions.Base.Extension.Init(_services, _services.BuildServiceProvider());

            _services.AddExtCore(_config[$"{core.Extensions.Base.Configuration.SectionRoot}:Folder"] != null ? $"{_env.ContentRootPath}{System.IO.Path.DirectorySeparatorChar}{_config[$"{core.Extensions.Base.Configuration.SectionRoot}:Folder"]}" : null);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public virtual void Configure(IApplicationBuilder app, IOptionsMonitor<TOptions> appConfigMonitor, IOptionsMonitor<Extensions.Base.Configuration> extConfigMonitor, IApplicationLifetime applicationLifetime)
        {
            //Error handling
            if (_env.IsDevelopment() || _env.IsEnvironment("Local") || (_config.GetSection("appConfig")?.GetValue<bool>("DeveloperExceptionPage") ?? false))
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseExtCore();

            Func<IDictionary<string, Extensions.Base.Configuration.Assembly>, string> _extSerialize = list => string.Join(" | ", list.Where(ext => ext.Value.Priority > 0).OrderBy(ext => ext.Value.Priority).Select(_ => _.Key));

            _extLastConfigAssembliesSerialized = _extSerialize(extConfigMonitor.CurrentValue.Assemblies);

            extConfigMonitor.OnChange(extConfig =>
            {

                var _extCurrentAssembliesSerialized = _extSerialize(extConfig.Assemblies);
                var isUpdatable = _extCurrentAssembliesSerialized == _extLastConfigAssembliesSerialized;

                _logger.CreateLogger("extConfigMonitor").LogWarning($"Config changed {DateTime.Now}; Is updatable: {isUpdatable} ");

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

        }

    }
}
