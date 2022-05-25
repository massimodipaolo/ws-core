using Carter;
using Carter.Response;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Ws.Core.Extensions.Diagnostic;

public class Extension : Base.Extension, ICarterModule
{
    private Options options => GetOptions<Options>() ?? new Options();

    public void AddRoutes(Microsoft.AspNetCore.Routing.IEndpointRouteBuilder app)
    {
        var _name = nameof(Diagnostic);
        var tag = $"{nameof(Extensions)}-{_name}".ToLower();
        var prefix = $"{nameof(Extensions)}/{_name}";
        app.MapGet($"{prefix}".ToLower(), AppRuntime.Get).WithTags(tag);
        app.MapPost($"{prefix}/{nameof(AppRuntime.Stop)}".ToLower(), AppRuntime.Stop).WithTags(tag);
        app.MapPost($"{prefix}/config/reload".ToLower(), AppRuntime.ReloadConfiguration).WithTags(tag);
    }

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
    }
}
