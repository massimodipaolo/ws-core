using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using NLog.Web;
using System.Reflection;

namespace Ws.Core;

public partial class Program
{
    protected Program() { }
    public static WebApplicationBuilder CreateBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            ContentRootPath = default,
            WebRootPath = "wwwroot",
            Args = args
        });
        builder.Host
            .ConfigureHostConfiguration(_ =>
            {
                _
                .AddJsonFile("host-settings.json", optional: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args); //i.e. --Environment=Local
            })
            .ConfigureLogging((ctx, logging) =>
            {
                logging.ClearProviders();
                logging.AddConfiguration(ctx.Configuration.GetSection("Logging")); // app-settings.json, app-settings.{env}.json
            })
            .ConfigureAppConfiguration((ctx, config) =>
            {
                var _env = ctx.HostingEnvironment;
                config
                    .SetBasePath(_env.ContentRootPath)
                    .AddJsonFile("app-settings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"app-settings.{_env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                    .AddJsonFile("log-settings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"log-settings.{_env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                    .AddJsonFile("ext-settings.json", optional: true, reloadOnChange: true) //IOptionsSnapshot to live reload              
                    .AddJsonFile($"ext-settings.{_env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                    .AddUserSecrets(Assembly.GetEntryAssembly(), optional: true) //override any config files    
                    .AddEnvironmentVariables() //override any user secrets
                    .AddCommandLine(args); //override any environment variables     
            });

        return builder;
    }

    public static void CreateRunBuilder<TOptions>(
        string[] args,
        Action<WebApplicationBuilder>? setupBuilder = null,
        Action<WebApplication>? setupApplication = null
        ) where TOptions : class, IAppConfiguration, new()
    {
        Logger? logger = null;
        try
        {
            // builder
            var builder = Ws.Core.Program.CreateBuilder(args);
            // logger
            logger = NLog.LogManager.Setup().LoadConfigurationFromSection(builder.Configuration)
                .SetupSerialization(s => s.RegisterJsonConverter(new LogJsonConverter()))
                .GetCurrentClassLogger();
            builder.Logging.ClearProviders();
            builder.Host.UseNLog();
            // startup
            var startup = new Ws.Core.Startup<TOptions>(builder);
            startup.Add(builder);
            setupBuilder?.Invoke(builder);
            // app
            var app = builder.Build();
            startup.Use(app);
            setupApplication?.Invoke(app);
            app.Run();
        }
        catch (Exception ex)
        {
            logger?.Fatal(ex, "Stopped program because of exception");
            throw;
        }
        finally
        {
            // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
            NLog.LogManager.Shutdown();
        }

    }
}
