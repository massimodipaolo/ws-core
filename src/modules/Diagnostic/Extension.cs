using Carter;
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
        var prefix = _route.prefix;
        var tag = _route.tag;
        app.MapGet($"{prefix}".ToLower(), AppRuntime.Get).WithTags(tag);
        app.MapPost($"{prefix}/{nameof(AppRuntime.Stop)}".ToLower(), AppRuntime.Stop).WithTags(tag);
    }

    public override void Execute(WebApplicationBuilder builder, IServiceProvider? serviceProvider = null)
    {
        _addHttpLogging(builder);
        _addProfiler(builder);
    }

    public override void Execute(WebApplication app)
    {
        if (_options.HttpLogging?.Enable == true)
            app.UseHttpLogging();

        if (_options.Profiler?.Enable == true)
            app.UseMiniProfiler();
    }

    private void _addHttpLogging(WebApplicationBuilder builder)
    {
        // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-logging/?view=aspnetcore-6.0
        if (_options.HttpLogging?.Enable == true)
            builder.Services.AddHttpLogging(_ =>
            {
                _.LoggingFields = _options.HttpLogging.Config.LoggingFields;
                _.RequestBodyLogLimit = _options.HttpLogging.Config.RequestBodyLogLimit;
                _.ResponseBodyLogLimit = _options.HttpLogging.Config.ResponseBodyLogLimit;
                foreach (var item in _options.HttpLogging.Config.RequestHeaders)
                    _.RequestHeaders.Add(item);
                foreach (var item in _options.HttpLogging.Config.ResponseHeaders)
                    _.ResponseHeaders.Add(item);
            });
    }

    private void _addProfiler(WebApplicationBuilder builder)
    {
        //MiniProfilerOptions: https://miniprofiler.com/dotnet/AspDotNetCore
        if (_options.Profiler?.Enable == true)
            builder.Services.AddMiniProfiler(_ =>
            {
                _.ColorScheme = _options.Profiler.Config.ColorScheme;
                _.EnableDebugMode = _options.Profiler.Config.EnableDebugMode;
                _.EnableServerTimingHeader = _options.Profiler.Config.EnableServerTimingHeader;
                foreach (var item in _options.Profiler.Config.ExcludedAssemblies)
                    _.ExcludedAssemblies.Add(item);
                foreach (var item in _options.Profiler.Config.ExcludedMethods)
                    _.ExcludedMethods.Add(item);
                foreach (var item in _options.Profiler.Config.ExcludedTypes)
                    _.ExcludedTypes.Add(item);
                foreach (var item in _options.Profiler.Config.IgnoredPaths)
                    _.IgnoredPaths.Add(item);
                _.MaxUnviewedProfiles = _options.Profiler.Config.MaxUnviewedProfiles;
                _.RouteBasePath = _options.Profiler.Config.RouteBasePath;
                _.ShowControls = _options.Profiler.Config.ShowControls;
                _.SqlFormatter = _options.Profiler.Config.SqlFormatter;
                _.StackMaxLength = _options.Profiler.Config.StackMaxLength;
            })
            .AddEntityFramework();
    }


}
