using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

Ws.Core.Program.CreateRunBuilder<Ws.Core.AppConfig>(
    x.core.Program.ParseArgs(args),
    x.core.Program.setupBuilder,
    x.core.Program.setupApplication
    );

namespace x.core
{
    public partial class Program : WebApplicationFactory<Program>
    {
        /// <summary>
        /// env variable                cli arg
        /// ------------------------------------------------------------------
        /// ASPNETCORE_ENVIRONMENT      --environment=Development
        /// ASPNETCORE_CONTENTROOT      --contentRoot=C:\Projects\ws-core\src\tests\x.core
        /// ASPNETCORE_APPLICATIONNAME  --applicationName=x.core
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        internal static string[] ParseArgs(string[] args)
        {
            // content root path
            var _path = "--contentRoot=";
            for (var i = 0; i < args.Length; i++)
                if (args[i].StartsWith(_path))
                    args[i] = $"{_path}{Path.GetFullPath(Directory.GetCurrentDirectory())}";
            return args;
        }

        internal static Action<WebApplicationBuilder> setupBuilder = (builder) => Add(builder);
        internal static Action<WebApplication> setupApplication = (app) => { Use(app); ConfigureServer(app); };

        internal static void Add(WebApplicationBuilder builder)
        {
            builder.Services.AddGrpc(_ =>
            {
                _.MaxReceiveMessageSize = 10 /*MB*/ * 1024 * 1024;
                _.MaxSendMessageSize = 100 /*MB*/ * 1024 * 1024;
            });

            // enrich services
            // builder.Services.AddTransient<Ws.Core.Extensions.Data.EF.DbContext, x.core.DbContextExt>();

            // override repo
            builder.Services.AddTransient(typeof(Ws.Core.Extensions.Data.IRepository<Models.Agenda, string>), typeof(Ws.Core.Extensions.Data.Repository.EF.MySql<Models.Agenda, string>));
            builder.Services.AddTransient(typeof(Ws.Core.Extensions.Data.IRepository<Models.CrudBase3, Guid>), typeof(Ws.Core.Extensions.Data.Repository.Mongo<Models.CrudBase3, Guid>));

            // override cache 
            builder.Services.AddSingleton(typeof(Ws.Core.Extensions.Data.Cache.ICache<Models.CrudBase2>), typeof(Ws.Core.Extensions.Data.Cache.SqlServer.SqlCache<Models.CrudBase2>));

            Ws.Core.Extensions.Api.Options? apiOpt = Ws.Core.Extensions.Base.Extension.Option<Ws.Core.Extensions.Api.Options>.Value;
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
        internal static void Use(WebApplication app)
        {
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

            app.MapGet("/foo", () => @"
██████╗  █████╗ ██████╗ 
██╔══██╗██╔══██╗██╔══██╗
██████╔╝███████║██████╔╝
██╔══██╗██╔══██║██╔══██╗
██████╔╝██║  ██║██║  ██║
╚═════╝ ╚═╝  ╚═╝╚═╝  ╚═╝");

            app.MapGrpcService<Services.Data>();
            app.MapGet("/api/data/event-log/{number}/{source}", x.core.Services.Data.GetEventLogDataApi);
            app.MapPost("/api/data/event-log/{destination}", x.core.Services.Data.PostEventLogDataApi);

        }

        /// <summary>
        /// Configure TestServer Services / Features
        /// </summary>
        /// <param name="app"></param>
        internal static void ConfigureServer(WebApplication app)
        {
            // IServerAddressesFeature
            // https://github.com/dotnet/aspnetcore/issues/5931#issuecomment-416307525
            // https://github.com/aspnet/Hosting/blob/3e0b689ac2ea72a8dee81f8ae3a349610ac1fb0c/test/Microsoft.AspNetCore.TestHost.Tests/TestServerTests.cs#L175-L176
            var server = app.Services?.GetService<IServer>();
            if (server?.Features != null && server.Features?.Get<IServerAddressesFeature>() == null)
            {
                var addressFeature = new ServerAddressesFeature();
                addressFeature.Addresses.Add("http://localhost:60936");
                addressFeature.Addresses.Add("https://localhost:60935");
                server?.Features?.Set<IServerAddressesFeature>(addressFeature);
            }
        }
    }
}