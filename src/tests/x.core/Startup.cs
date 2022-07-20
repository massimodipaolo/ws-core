using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;

namespace x.core;

public class Startup : Ws.Core.Startup<Ws.Core.AppConfig>
{
    public Startup(WebApplicationBuilder builder) : base(builder.Environment, builder.Services.BuildServiceProvider().GetRequiredService<IConfiguration>()) { }

    public void Add(WebApplicationBuilder builder) => ConfigureServices(builder);

    public override void ConfigureServices(WebApplicationBuilder builder)
    {
        base.ConfigureServices(builder);        
        Ws.Core.AppInfo<Ws.Core.AppConfig>.ConfigureServices(env: env, config: config, services: builder.Services);

        builder.Services.AddGrpc(_ =>
        {
            _.MaxReceiveMessageSize = 10 /*MB*/ * 1024 * 1024; 
            _.MaxSendMessageSize = 100 /*MB*/ * 1024 * 1024; 
        });

        // override repo
        builder.Services.AddTransient(typeof(Ws.Core.Extensions.Data.IRepository<Models.Agenda, string>), typeof(Ws.Core.Extensions.Data.Repository.EF.MySql<Models.Agenda, string>));
        builder.Services.AddTransient(typeof(Ws.Core.Extensions.Data.IRepository<Models.CrudBase3, Guid>), typeof(Ws.Core.Extensions.Data.Repository.Mongo<Models.CrudBase3, Guid>));

        // override cache 
        builder.Services.AddSingleton(typeof(Ws.Core.Extensions.Data.Cache.ICache<Models.CrudBase2>), typeof(Ws.Core.Extensions.Data.Cache.SqlServer.SqlCache<Models.CrudBase2>));       

        Ws.Core.Extensions.Api.Options apiOpt = Ws.Core.Extensions.Base.Extension.Option<Ws.Core.Extensions.Api.Options>.Value;
        builder.Services.Configure<JsonOptions>(_ => {
            var opt = apiOpt?.Serialization?.ToJsonSerializerSettings();
            if (opt != null)
            {

                _.SerializerOptions.AllowTrailingCommas = opt.AllowTrailingCommas;
                foreach (var converter in opt.Converters)
                    _.SerializerOptions.Converters.Add(converter);
                _.SerializerOptions.DefaultIgnoreCondition = opt.DefaultIgnoreCondition;
                _.SerializerOptions.PropertyNameCaseInsensitive = opt.PropertyNameCaseInsensitive;
                _.SerializerOptions.PropertyNamingPolicy = opt.PropertyNamingPolicy;
                _.SerializerOptions.WriteIndented = opt.WriteIndented;
            }
        });

    }

    public void Use(WebApplication app)
    {
        var services = app.Services;

        app.MapGet("/api/log", (Ws.Core.Extensions.Data.IRepository<Log, int> repo) => repo.List.FirstOrDefault());
        app.MapGet("/api/log/{id}", (int id, Ws.Core.Extensions.Data.IRepository<Log, int> repo) => repo.Find(id));

        app.MapGet("/message/send", async (Ws.Core.Extensions.Message.IMessage svc) => {
            var sender = "massimodipaolo@users.noreply.github.com";
            var recipient = "massimo.dipaolo@mail.local";
            var content = $"{typeof(Ws.Core.Extensions.Message.IMessage)} => {svc.GetType().FullName}";
            var message = new Ws.Core.Extensions.Message.Message()
            {
                Sender = new Ws.Core.Extensions.Message.Message.Actor() { Address = sender, Name = sender },
                Recipients = new Ws.Core.Extensions.Message.Message.Actor[]
                {
                    new() { Address = recipient, Name = recipient, Type = Ws.Core.Extensions.Message.Message.ActorType.Primary }
                },
                Subject = $"Decorators 🎍 demo 👹",
                Content = content, 
                Format = "html"
            };
            await svc.SendAsync(message, throwException: true);
            return content;
        });
        
        Configure(
            app,
            services.GetRequiredService<IOptionsMonitor<Ws.Core.AppConfig>>(),
            services.GetRequiredService<IOptionsMonitor<Ws.Core.Extensions.Base.Configuration>>(),
            app.Lifetime,
            services.GetRequiredService<ILogger<Ws.Core.Program>>()
            );

        app.MapGrpcService<Services.Data>();
        app.MapGet("/api/data/event-log/{number}/{source}", x.core.Services.Data.GetEventLogDataApi);
        app.MapPost("/api/data/event-log/{destination}", x.core.Services.Data.PostEventLogDataApi);

    }
    public override void Configure(WebApplication app, IOptionsMonitor<Ws.Core.AppConfig> appConfigMonitor, IOptionsMonitor<Ws.Core.Extensions.Base.Configuration> extConfigMonitor, IHostApplicationLifetime lifetime, ILogger<Ws.Core.Program> logger)
    {
        logger.LogInformation("Start");

        Ws.Core.AppInfo<Ws.Core.AppConfig>.ConfigureApp(app: app, appConfigMonitor: appConfigMonitor, extConfigMonitor: extConfigMonitor, loggerFactory: app.Services?.GetRequiredService<ILoggerFactory>(), lifetime: lifetime);

        base.Configure(app, appConfigMonitor, extConfigMonitor, lifetime, logger);
        
        app.MapGet("/foo", () => @"
██████╗  █████╗ ██████╗ 
██╔══██╗██╔══██╗██╔══██╗
██████╔╝███████║██████╔╝
██╔══██╗██╔══██║██╔══██╗
██████╔╝██║  ██║██║  ██║
╚═════╝ ╚═╝  ╚═╝╚═╝  ╚═╝");

        //shutdown
        lifetime.ApplicationStopping.Register(() =>
        {
            logger.LogInformation("Shutdown");
        }
        );
    }

}
