using Carter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
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
    public Startup(IWebHostEnvironment hostingEnvironment, IConfiguration configuration)
    {
        env = hostingEnvironment;
        config = configuration;
    }

    public static readonly DateTime Uptime = DateTime.Now;
    protected string appConfigSectionRoot { get; set; } = nameof(AppConfig);
    protected IWebHostEnvironment env { get; set; }
    protected IConfiguration config { get; set; }

    public static readonly Func<IConfiguration, IWebHostEnvironment, string?> ExtCoreExtensionsPath = (config, env)
        => config[$"{Extensions.Base.Configuration.SectionRoot}:Folder"] != null
        ? $"{env.ContentRootPath}{System.IO.Path.DirectorySeparatorChar}{config[$"{Extensions.Base.Configuration.SectionRoot}:Folder"]}"
        : null;
}
public class Startup<TOptions> : Startup where TOptions : class, IAppConfiguration, new()
{
    public Startup(WebApplicationBuilder builder) : base(builder.Environment, builder.Services.BuildServiceProvider().GetRequiredService<IConfiguration>()) { }

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public virtual void Add(WebApplicationBuilder builder) => _add(builder);
    private void _add(WebApplicationBuilder builder)
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
        if (carterModules?.ToList()?.Any() == true)
            builder.Services.AddCarter(configurator: _ => _
                .WithModules(carterModules.ToArray())
                );

        AppInfo.ConfigureServices(env, config, services: builder.Services);
    }

    public virtual void Use(WebApplication app)
    {
        var services = app.Services;
        _use(
            app,
            services.GetRequiredService<IOptionsMonitor<TOptions>>(),
            services.GetRequiredService<IOptionsMonitor<Ws.Core.Extensions.Base.Configuration>>(),
            app.Lifetime,
            services.GetRequiredService<ILogger<Ws.Core.Program>>()
        );
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    private void _use(WebApplication app, IOptionsMonitor<TOptions> appConfigMonitor, IOptionsMonitor<Extensions.Base.Configuration> extConfigMonitor, IHostApplicationLifetime lifetime, ILogger<Ws.Core.Program> logger)
    {

        //Error handling
        if (env.IsDevelopment() || env.EnvironmentName == "Local" || (appConfigMonitor.CurrentValue?.DeveloperExceptionPage ?? false))
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseExtCore();

        app.MapCarter();
        

        Ws.Core.AppInfo<TOptions>.ConfigureApp(app: app, appConfigMonitor, extConfigMonitor, loggerFactory: app.Services?.GetRequiredService<ILoggerFactory>(), lifetime);

        //lifetime events
        lifetime.ApplicationStarted.Register(() => logger.LogInformation("Startup"));
        lifetime.ApplicationStopping.Register(() => logger.LogInformation("Shutdown"));

    }


}
