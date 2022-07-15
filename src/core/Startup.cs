using Carter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ws.Core.Extensions;
using Ws.Core.Extensions.Base;

namespace Ws.Core;

public class Startup
{
    protected Startup() { }
    public static readonly DateTime Uptime = DateTime.Now;
    protected string appConfigSectionRoot { get; set; } = nameof(AppConfig);
    protected IWebHostEnvironment env { get; set; }
    protected IConfiguration config;
    public static readonly Func<IConfiguration, IWebHostEnvironment, string> ExtCoreExtensionsPath = (config, env)
        => config[$"{Extensions.Base.Configuration.SectionRoot}:Folder"] != null
        ? $"{env.ContentRootPath}{System.IO.Path.DirectorySeparatorChar}{config[$"{Extensions.Base.Configuration.SectionRoot}:Folder"]}"
        : null;
}
public class Startup<TOptions> : Startup where TOptions : class, IAppConfiguration
{
    private string _extLastConfigAssembliesSerialized { get; set; }

    public Startup(IWebHostEnvironment hostingEnvironment, IConfiguration configuration)
    {
        env = hostingEnvironment;
        config = configuration;
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public virtual void ConfigureServices(WebApplicationBuilder builder)
    {
        var services = builder.Services;

        services.AddOptions();

        services.AddSingleton(config);

        services.Configure<Configuration>(config.GetSection(Core.Extensions.Base.Configuration.SectionRoot));

        services.Configure<TOptions>(config.GetSection(appConfigSectionRoot));

        services.AddSingleton<IAppConfiguration, TOptions>();

        Extensions.Base.Extension.Init(services, services.BuildServiceProvider());

        builder.AddExtCore(ExtCoreExtensionsPath(config, env), includingSubpaths: true);

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

            logger.LogInformation("Config changed; Is updatable: {isUpdatable} ", isUpdatable);

            if (!isUpdatable && extConfig.EnableShutDownOnChange)
                lifetime.StopApplication();
        });

    }

}
