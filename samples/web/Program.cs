using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using NLog;
using NLog.Web;
using System;
using System.Linq;

var logger = NLog.LogManager.Setup().LoadConfigurationFromFile("NLog.config").GetCurrentClassLogger();
logger.Debug("Init main");

try
{
    var builder = Ws.Core.Program.CreateBuilder(args); 
    builder.Host.UseNLog();

    var startup = new web.Startup(builder);
    startup.Add(builder);
    var app = builder.Build();

    startup.Use(app);

    app.MapGet("/builder", () => Newtonsoft.Json.JsonConvert.SerializeObject(
        new
        {
            builder.Environment,
            builder.Configuration,
            builder.Host,
            builder.WebHost,
            services = builder.Services.AsEnumerable().OrderBy(_ => _.ServiceType.ToString()).ThenBy(_ => _.ImplementationType?.ToString())
            .Select(_ => new {
                ServiceType = _.ServiceType?.ToString(),
                ImplementationType = _.ImplementationType?.ToString(),
                ImplementationInstance = _.ImplementationInstance?.ToString(),
                Lifetime = _.Lifetime.ToString()
            })
        },
        Newtonsoft.Json.Formatting.Indented,
        settings: new Newtonsoft.Json.JsonSerializerSettings() { ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore })
    );

    app.Run();
}
catch (Exception ex)
{
    logger.Fatal(ex, "Stopped program");
    throw;
}
finally
{
    NLog.LogManager.Shutdown();
}


namespace web
{
    public partial class Program {}
}
