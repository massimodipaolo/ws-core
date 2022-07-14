using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
                    .AddJsonFile("ext-settings.json", optional: true, reloadOnChange: true) //IOptionsSnapshot to live reload              
                    .AddJsonFile($"ext-settings.{_env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                    .AddUserSecrets(Assembly.GetEntryAssembly(), optional: true) //override any config files    
                    .AddEnvironmentVariables() //override any user secrets
                    .AddCommandLine(args); //override any environment variables     
            });
        return builder;
    }
}
