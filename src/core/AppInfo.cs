using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ws.Core
{
    public class AppInfo
    {
        protected AppInfo() { }
        public static IWebHostEnvironment Env { get; protected set; }
        public static IConfiguration Config { get; protected set; }
        public static IApplicationBuilder App { get; protected set; }
        public static IServiceCollection Services { get; protected set; }
        public static IServiceProvider ServiceProvider { get; protected set; }
        public static ILoggerFactory LoggerFactory { get; protected set; } = App?.ApplicationServices?.GetRequiredService<ILoggerFactory>();
        public static ILogger<T> Logger<T>() where T : class => LoggerFactory?.CreateLogger<T>();
        public static ILogger Logger(Type type) => LoggerFactory?.CreateLogger(type);
        public static ILogger Logger(string name) => LoggerFactory?.CreateLogger(name);
        public static IHostApplicationLifetime Lifetime { get; protected set; }
        public static IHttpContextAccessor HttpContextAccessor => ServiceProvider.GetService<IHttpContextAccessor>();
        public static IOptionsMonitor<Core.Extensions.Base.Configuration> ExtConfigMonitor { get; protected set; }
    }
    public class AppInfo<TConfig>: AppInfo where TConfig: class, IAppConfiguration, new()
    {
        public static IOptionsMonitor<TConfig> AppConfigMonitor { get; private set; } 
        public static IOptions<TConfig> AppConfig { get; private set; }
        public static void ConfigureServices(
            IWebHostEnvironment env = null,
            IConfiguration config = null,
            IServiceCollection services = null
            )
        {
            if (env != null) Env = env;
            if (config != null) Config = config;
            if (services != null)
            {
                Services = services;
                ServiceProvider = Services.BuildServiceProvider();
            }
        }
        public static void ConfigureApp(
            IApplicationBuilder app = null,
            IOptionsMonitor<TConfig> appConfigMonitor = null,
            IOptionsMonitor<Core.Extensions.Base.Configuration> extConfigMonitor = null,
            ILoggerFactory loggerFactory = null,
            IHostApplicationLifetime lifetime = null
            )
        {
            if (app != null) App = app;
            if (appConfigMonitor != null)
            {
                AppConfigMonitor = appConfigMonitor;
                AppConfig = ServiceProvider?.GetService<IOptions<TConfig>>();
            }
            if (extConfigMonitor != null) ExtConfigMonitor = extConfigMonitor;
            if (loggerFactory != null) LoggerFactory = loggerFactory;
            if (lifetime != null) Lifetime = lifetime;
        }
    }
}
