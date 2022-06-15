using Microsoft.AspNetCore.Builder;
using System;
using SixLabors.ImageSharp.Web.DependencyInjection;
using SixLabors.ImageSharp.Web.Commands;
using SixLabors.ImageSharp.Web.Caching;
using SixLabors.ImageSharp.Web.Providers;
using SixLabors.ImageSharp.Web.Processors;
using SixLabors.ImageSharp;
using System.Threading.Tasks;

namespace Ws.Core.Extensions.ImageProcessor;

public class Extension : Base.Extension
{

    public override void Execute(WebApplicationBuilder builder, IServiceProvider serviceProvider = null)
    {
        builder.Services.AddImageSharp(options =>
        {
            options.Configuration = Configuration.Default;
            options.BrowserMaxAge = TimeSpan.FromDays(7);
            options.CacheMaxAge = TimeSpan.FromDays(365);
            options.CachedNameLength = 12;
            options.OnParseCommandsAsync = _ => Task.CompletedTask;
            options.OnBeforeSaveAsync = _ => Task.CompletedTask;
            options.OnProcessedAsync = _ => Task.CompletedTask;
            options.OnPrepareResponseAsync = _ => Task.CompletedTask;
        })
                .SetRequestParser<QueryCollectionRequestParser>()
                .Configure<PhysicalFileSystemCacheOptions>(options =>
                {
                    options.CacheRoot = "wwwroot";
                    options.CacheFolder = "is-cache";
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
    public override void Execute(WebApplication app)
    {
        app.UseImageSharp();
    }
}
