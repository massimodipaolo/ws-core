using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.Testing;

var builder = Ws.Core.Program.CreateBuilder(x.core.Program.ParseArgs(args));
builder.Logging.AddConsole();
var startup = new x.core.Startup(builder);
startup.Add(builder);
var app = builder.Build();
x.core.Program.ConfigureServer(app);
startup.Use(app);
app.Run();

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

        /// <summary>
        /// Configure TestServer Services / Features
        /// </summary>
        /// <param name="app"></param>
        internal static void ConfigureServer(WebApplication app)
        {
            // IServerAddressesFeature
            // https://github.com/dotnet/aspnetcore/issues/5931#issuecomment-416307525
            // https://github.com/aspnet/Hosting/blob/3e0b689ac2ea72a8dee81f8ae3a349610ac1fb0c/test/Microsoft.AspNetCore.TestHost.Tests/TestServerTests.cs#L175-L176
            var server = app.Services.GetRequiredService<IServer>();
            if (server.Features.Get<IServerAddressesFeature>() == null)
            {
                var addressFeature = new ServerAddressesFeature();
                addressFeature.Addresses.Add("http://localhost:60936");
                addressFeature.Addresses.Add("https://localhost:60935");
                server.Features.Set<IServerAddressesFeature>(addressFeature);
            }
        }
    }
}