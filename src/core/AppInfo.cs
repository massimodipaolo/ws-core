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
    public class AppInfo<TConfig> where TConfig: class, IAppConfiguration, new()
    {
        public static IWebHostEnvironment Env { get; private set; }
        public static IConfiguration Config { get; private set; }
        public static ILoggerFactory LoggerFactory { get; private set; }
        public static ILogger<T> Logger<T>() where T : class => LoggerFactory.CreateLogger<T>();
        public static ILogger Logger(Type type) => LoggerFactory.CreateLogger(type);
        public static ILogger Logger(string name) => LoggerFactory.CreateLogger(name);
        public static IApplicationBuilder App { get; private set; }
        public static IServiceCollection Services { get; set; }
        public static IServiceProvider ServiceProvider { get; set; }
        public static IOptionsMonitor<TConfig> AppConfigMonitor { get; private set; } 
        public static IOptions<TConfig> AppConfig { get; private set; }
        public static IOptionsMonitor<Core.Extensions.Base.Configuration> ExtConfigMonitor { get; private set; }
        public static IHostApplicationLifetime ApplicationLifetime { get; private set; }
        public static IHttpContextAccessor HttpContextAccessor => ServiceProvider.GetService<IHttpContextAccessor>();
        public static void Set(
            IWebHostEnvironment env = null,
            IConfiguration config = null,
            ILoggerFactory loggerFactory = null,
            IApplicationBuilder app = null,
            IServiceCollection services = null,
            IOptionsMonitor<TConfig> appConfigMonitor = null,
            IOptionsMonitor<Core.Extensions.Base.Configuration> extConfigMonitor = null,
            IHostApplicationLifetime applicationLifetime = null
            )
        {
            if (env != null) Env = env;
            if (config != null) Config = config;
            if (loggerFactory != null) LoggerFactory = loggerFactory;
            if (app != null) App = app;
            if (services != null)
            {
                Services = services;
                ServiceProvider = Services.BuildServiceProvider();
            }
            if (appConfigMonitor != null)
            {
                AppConfigMonitor = appConfigMonitor;
                AppConfig = ServiceProvider?.GetService<IOptions<TConfig>>();
            }
            if (extConfigMonitor != null) ExtConfigMonitor = extConfigMonitor;
            if (applicationLifetime != null) ApplicationLifetime = applicationLifetime;
        }
    }
}
