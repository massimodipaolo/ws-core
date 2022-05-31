﻿using Carter;
using Microsoft.Extensions.Options;

namespace xCore;

public class Startup : Ws.Core.Startup<Ws.Core.AppConfig>
{
    public Startup(WebApplicationBuilder builder) : base(builder.Environment, (IConfiguration)builder.Services.BuildServiceProvider().GetRequiredService<IConfiguration>()) { }

    public void Add(WebApplicationBuilder builder) => ConfigureServices(builder);

    public override void ConfigureServices(WebApplicationBuilder builder)
    {
        base.ConfigureServices(builder);        
        Ws.Core.AppInfo<Ws.Core.AppConfig>.Set(env: env, config: config, services: builder.Services);

        builder.Services.AddGrpc(_ =>
        {
            _.MaxReceiveMessageSize = 10 /*MB*/ * 1024 * 1024; 
            _.MaxSendMessageSize = 100 /*MB*/ * 1024 * 1024; 
        });

        /*
        builder.Services
            .Decorate<Ws.Core.Extensions.Message.IMessage, xCore.Decorators.IMessageLogger>()
            .Decorate<Ws.Core.Extensions.Message.IMessage, xCore.Decorators.IMessageCopy>()
            .Decorate<Ws.Core.Extensions.Message.IMessage, xCore.Decorators.IMessageSignature>()
            ; 
        */
        //builder.Services.AddTransient(typeof(Ws.Core.Extensions.Data.IRepository<Log,int>), typeof(Ws.Core.Extensions.Data.Repository.InMemory<Log, int>));

        // override dbContext
        //builder.Services.AddTransient<Ws.Core.Extensions.Data.EF.SQLite.DbContext, xCore.AppEmbeddedDbContextExt>();

        // override repo
        builder.Services.AddTransient(typeof(Ws.Core.Extensions.Data.IRepository<Endpoints.Agenda, string>), typeof(Ws.Core.Extensions.Data.Repository.EF.MySql<Endpoints.Agenda, string>));
        /*
        builder.Services.AddTransient(typeof(Ws.Core.Extensions.Data.IRepository<Models.User,int>), typeof(Ws.Core.Extensions.Data.Repository.FileSystem<Models.User,int>));
        builder.Services.AddTransient(typeof(Ws.Core.Extensions.Data.IRepository<Models.Album,int>), typeof(Ws.Core.Extensions.Data.Repository.EF.SqlServer<Models.Album,int>));
        builder.Services.AddTransient(typeof(Ws.Core.Extensions.Data.IRepository<Models.Photo,int>), typeof(Ws.Core.Extensions.Data.Repository.EF.SqlServer<Models.Photo,int>));
        builder.Services.AddTransient(typeof(Ws.Core.Extensions.Data.IRepository<Models.Post,int>), typeof(Ws.Core.Extensions.Data.Repository.EF.SqlServer<Models.Post,int>));
        builder.Services.AddTransient(typeof(Ws.Core.Extensions.Data.IRepository<Models.Comment,int>), typeof(Ws.Core.Extensions.Data.Repository.EF.SqlServer<Models.Comment,int>));
        */

        var carterModules = Ws.Core.Extensions.Base.Util.GetAllTypesOf<ICarterModule>();
        if (carterModules.Any())
            builder.Services.AddCarter(configurator: _ => _
                .WithModules(carterModules.ToArray())
                );
    }

    public void Use(WebApplication app)
    {
        var services = app.Services;

        app.MapGet("/api/log", (Ws.Core.Extensions.Data.IRepository<Log, int> repo) => repo.List.FirstOrDefault());
        app.MapGet("/api/log/{id}", (int id, Ws.Core.Extensions.Data.IRepository<Log, int> repo) => repo.Find(id));

        app.MapGet("/message/send", async (Ws.Core.Extensions.Message.IMessage svc) => {
            var email = "massimodipaolo@users.noreply.github.com";
            var content = $"{typeof(Ws.Core.Extensions.Message.IMessage)} => {svc.GetType().FullName}";
            var message = new Ws.Core.Extensions.Message.Message()
            {
                Sender = new Ws.Core.Extensions.Message.Message.Actor() { Address = email, Name = email },
                Recipients = new Ws.Core.Extensions.Message.Message.Actor[]
                {
                    new() { Address = email, Name = email, Type = Ws.Core.Extensions.Message.Message.ActorType.Primary }
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
        app.MapGet("/api/data/event-log/{number}/{source}", xCore.Services.Data.GetEventLogDataApi);
        app.MapPost("/api/data/event-log/{destination}", xCore.Services.Data.PostEventLogDataApi);

        app.MapCarter();
    }
    public override void Configure(WebApplication app, IOptionsMonitor<Ws.Core.AppConfig> appConfigMonitor, IOptionsMonitor<Ws.Core.Extensions.Base.Configuration> extConfigMonitor, IHostApplicationLifetime lifetime, ILogger<Ws.Core.Program> logger)
    {
        logger.LogInformation("Start");

        Ws.Core.AppInfo<Ws.Core.AppConfig>.Set(app: app, appConfigMonitor: appConfigMonitor, extConfigMonitor: extConfigMonitor, loggerFactory: app.Services?.GetRequiredService<ILoggerFactory>(), lifetime: lifetime);

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
