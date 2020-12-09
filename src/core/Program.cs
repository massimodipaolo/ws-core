using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Ws.Core
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            //BuildWebHost(args, typeof(Program).Assembly, typeof(Startup<AppConfig>)).Run();
            await Task.CompletedTask;
        }

        public static IHostBuilder WebHostBuilder(string[] args, Assembly assembly, System.Type startup) =>
        new HostBuilder()
        //Host.CreateDefaultBuilder(args)
                    .ConfigureWebHostDefaults(_ =>
                    {
                        //_.UseKestrel((ctx, opt) => { opt.AddServerHeader = false; });
                        //_.UseIIS();
                        _.UseContentRoot(Directory.GetCurrentDirectory());
                        _.UseStartup(startup);                        
                    })
                    .ConfigureLogging((ctx, logging) =>
                    {
                        logging.ClearProviders();
                        logging.AddConfiguration(ctx.Configuration.GetSection("Logging"));
                        if (ctx.HostingEnvironment.EnvironmentName == "Development" || ctx.HostingEnvironment.EnvironmentName == "Local")
                        {
                            logging.AddDebug();
                        }
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
            .AddUserSecrets(assembly, optional: true) //override any config files    
            .AddEnvironmentVariables() //override any user secrets
            .AddCommandLine(args); //override any environment variables     
        })
           ;

        public static IHost BuildWebHost(string[] args, Assembly assembly, System.Type startup) =>
            WebHostBuilder(args, assembly, startup)
            .Build();


    }
}
