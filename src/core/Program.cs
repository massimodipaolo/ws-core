using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace core
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args, typeof(Program).Assembly).Run();
        }

        public static IWebHost BuildWebHost(string[] args, Assembly assembly) =>
            WebHostBuilder(args, assembly)
            .UseStartup<Startup<AppConfig>>()
            .Build();

        public static IWebHostBuilder WebHostBuilder(string[] args, Assembly assembly) =>
            new WebHostBuilder()
            .UseKestrel((ctx, opt) => { opt.AddServerHeader = false; })
            //.ConfigureKestrel((ctx,opt) => { opt.AddServerHeader = false; }) // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel?view=aspnetcore-2.2#how-to-use-kestrel-in-aspnet-core-apps
            .UseContentRoot(Directory.GetCurrentDirectory())     // issue with in-process hosting https://github.com/aspnet/AspNetCore/issues/4206                   
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
            .ConfigureLogging((ctx, logging) =>
            {
                logging.AddConfiguration(ctx.Configuration.GetSection("Logging"));
                if (ctx.HostingEnvironment.IsDevelopment() || ctx.HostingEnvironment.IsEnvironment("Local"))
                {
                    logging.AddDebug();
                }
            });


    }
}
