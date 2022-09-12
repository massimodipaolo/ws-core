using Microsoft.AspNetCore.Builder;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Web.Caching;
using SixLabors.ImageSharp.Web.Commands;
using SixLabors.ImageSharp.Web.DependencyInjection;
using SixLabors.ImageSharp.Web.Processors;
using SixLabors.ImageSharp.Web.Providers;

namespace Ws.Core.Extensions.ImageProcessor;

public class Extension : Base.Extension
{
    private Options options => GetOptions<Options>() ?? new Options();
    public override void Add(WebApplicationBuilder builder, IServiceProvider? serviceProvider = null)
    {
        builder.Services.AddImageSharp(o =>
{
o.Configuration = options.Config?.Configuration ?? Configuration.Default;
o.BrowserMaxAge = options.Config?.BrowserMaxAge ?? TimeSpan.FromDays(7);
o.CacheMaxAge = options.Config?.CacheMaxAge ?? TimeSpan.FromDays(365);
o.CachedNameLength = options.Config?.CachedNameLength ?? 8;
o.OnParseCommandsAsync = _ => Task.CompletedTask;
o.OnBeforeSaveAsync = _ => Task.CompletedTask;
o.OnProcessedAsync = _ => Task.CompletedTask;
o.OnPrepareResponseAsync = _ => Task.CompletedTask;
})
        .SetRequestParser<QueryCollectionRequestParser>()
        .Configure<PhysicalFileSystemCacheOptions>(o =>
        {
            o.CacheRoot = options.FileSystemCache?.CacheRoot ?? "wwwroot";
            o.CacheFolder = options.FileSystemCache?.CacheFolder ?? "is-cache";
        })
        .SetCache<PhysicalFileSystemCache>()
        .ClearProviders()
        .AddProvider<PhysicalFileSystemProvider>()
        .ClearProcessors()
        .AddProcessor<ResizeWebProcessor>()
        .AddProcessor<FormatWebProcessor>()
        .AddProcessor<BackgroundColorWebProcessor>()
        .AddProcessor<JpegQualityWebProcessor>();
    }
    public override void Use(WebApplication app)
    {
        app.UseImageSharp();
    }
}
