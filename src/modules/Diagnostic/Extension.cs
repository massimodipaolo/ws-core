using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Ws.Core.Extensions.Diagnostic;

public class Extension: Base.Extension
{
    private Options options => GetOptions<Options>() ?? new Options();
    public override void Execute(WebApplicationBuilder builder, IServiceProvider? serviceProvider = null)
    {
        // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-logging/?view=aspnetcore-6.0
        builder.Services.AddHttpLogging(_ => _ = options.httpLogging);

        //MiniProfilerOptions: https://miniprofiler.com/dotnet/AspDotNetCore
        builder.Services.AddMiniProfiler(_ => _ = options.profiler).AddEntityFramework();
    }

    public override void Execute(WebApplication app)
    {
        app.UseHttpLogging();
        app.UseMiniProfiler();


        //app.Map("/api", _ => _.UseMiddleware<TelemetryMiddleware>());
        //app.UseMiddleware<TelemetryMiddleware>();

#warning TODO : here Extensions.Api diagnostic (GET e /stop)
        app.MapPost("/configuration/reload", (Microsoft.Extensions.Configuration.IConfiguration config) =>
        {
            if (config is Microsoft.Extensions.Configuration.IConfigurationRoot root)
            {
                root.Reload();
            }
            return Microsoft.AspNetCore.Http.Results.NoContent();
        });
    }
}
