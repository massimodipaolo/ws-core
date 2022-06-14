using Carter;
using Carter.Response;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Ws.Core.Extensions.Diagnostic;

public class Extension : Base.Extension, ICarterModule
{
    private Options _options => GetOptions<Options>() ?? new Options();
    private (string prefix, string tag) _route => GetApiRoute();
    public void AddRoutes(Microsoft.AspNetCore.Routing.IEndpointRouteBuilder app)
    {
        /*
        var _name = nameof(Diagnostic);
        var tag = $"{nameof(Extensions)}-{_name}".ToLower();
        var prefix = $"{nameof(Extensions)}/{_name}";
        */
        var prefix = _route.prefix;
        var tag = _route.tag;
        app.MapGet($"{prefix}".ToLower(), AppRuntime.Get).WithTags(tag);
        app.MapPost($"{prefix}/{nameof(AppRuntime.Stop)}".ToLower(), AppRuntime.Stop).WithTags(tag);
        app.MapPost($"{prefix}/config/reload".ToLower(), AppRuntime.ReloadConfiguration).WithTags(tag);
    }

    public override void Execute(WebApplicationBuilder builder, IServiceProvider? serviceProvider = null)
    {
        // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-logging/?view=aspnetcore-6.0
        if (_options.HttpLogging?.Enable == true)
            builder.Services.AddHttpLogging(_ => _ = _options.HttpLogging.Config);

        //MiniProfilerOptions: https://miniprofiler.com/dotnet/AspDotNetCore
        if (_options.Profiler?.Enable == true)
            builder.Services.AddMiniProfiler(_ => _ = _options.Profiler.Config).AddEntityFramework();
    }

    public override void Execute(WebApplication app)
    {
        if (_options.HttpLogging?.Enable == true)
            app.UseHttpLogging();

        if (_options.Profiler?.Enable == true)
            app.UseMiniProfiler();
    }
}
