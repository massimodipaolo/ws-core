using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Ws.Core.Extensions.Diagnostic;

public class Extension : Base.Extension
{
    private Options options => GetOptions<Options>() ?? new Options();
    public override void Execute(WebApplicationBuilder builder, IServiceProvider? serviceProvider = null)
    {
        // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-logging/?view=aspnetcore-6.0
        if (options.HttpLogging?.Enable == true)
            builder.Services.AddHttpLogging(_ => _ = options.HttpLogging.Config);

        //MiniProfilerOptions: https://miniprofiler.com/dotnet/AspDotNetCore
        if (options.Profiler?.Enable == true)
            builder.Services.AddMiniProfiler(_ => _ = options.Profiler.Config).AddEntityFramework();
    }

    public override void Execute(WebApplication app)
    {
        if (options.HttpLogging?.Enable == true)
            app.UseHttpLogging();

        if (options.Profiler?.Enable == true)
            app.UseMiniProfiler();

#warning TODO : move here Extensions.Api diagnostic (GET e /stop)
        /*
        app.MapPost("/configuration/reload", (Microsoft.Extensions.Configuration.IConfiguration config) =>
        {
            if (config is Microsoft.Extensions.Configuration.IConfigurationRoot root)
            {
                root.Reload();
            }
            return Microsoft.AspNetCore.Http.Results.NoContent();
        });
        */
    }
}
