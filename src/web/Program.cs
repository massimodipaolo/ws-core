using Carter;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

Ws.Core.Program.CreateRunBuilder<Ws.Core.AppConfig>(
    ws.bom.oven.web.Program.ParseArgs(args),
    ws.bom.oven.web.Program.setupBuilder,
    ws.bom.oven.web.Program.setupApplication
    );

namespace ws.bom.oven.web
{
    public partial class Program : WebApplicationFactory<Program>, ICarterModule
    {
        /// <summary>
        /// env variable                cli arg
        /// ------------------------------------------------------------------
        /// ASPNETCORE_ENVIRONMENT      --environment=Development
        /// ASPNETCORE_CONTENTROOT      --contentRoot=C:\Projects\ws-core\src\oven\web
        /// ASPNETCORE_APPLICATIONNAME  --applicationName=ws.bom.oven.web
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

        internal static readonly Action<WebApplicationBuilder> setupBuilder = (builder) => Add(builder);
        internal static readonly Action<WebApplication> setupApplication = (app) => { Use(app); };

        internal static void Add(WebApplicationBuilder builder)
        {
            builder.Services.AddResponseCompression();
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
            ConfigureServer(app);
        }

        /// <summary>
        /// Configure TestServer Services / Features
        /// </summary>
        /// <param name="app"></param>
        internal static void ConfigureServer(WebApplication app)
        {
            app.UseResponseCompression();
            // IServerAddressesFeature
            // https://github.com/dotnet/aspnetcore/issues/5931#issuecomment-416307525
            // https://github.com/aspnet/Hosting/blob/3e0b689ac2ea72a8dee81f8ae3a349610ac1fb0c/test/Microsoft.AspNetCore.TestHost.Tests/TestServerTests.cs#L175-L176
            var server = app.Services?.GetService<IServer>();
            if (server?.Features != null && server.Features?.Get<IServerAddressesFeature>() == null)
            {
                var addressFeature = new ServerAddressesFeature();
                addressFeature.Addresses.Add("http://localhost:5000");
                addressFeature.Addresses.Add("https://localhost:5001");
                server?.Features?.Set<IServerAddressesFeature>(addressFeature);
            }
        }

        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/", () => string.Empty);
        }
    }
}