using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;

namespace Ws.Core.Extensions.Spa
{
    public class Extension : Base.Extension
    {
        private Options options => GetOptions<Options>();

        public override void Execute(WebApplicationBuilder builder, IServiceProvider serviceProvider = null)
        {
            base.Execute(builder, serviceProvider);

            try
            {
                if (options != null)
                {
                    builder.Services.AddSpaStaticFiles(_ =>
                    {
                        _.RootPath = options.RootPath;
                    });
                    _addDiscriminator(builder, options);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "");
            }

        }

        public override void Execute(WebApplication app)
        {
            base.Execute(app);

            try
            {
                if (options != null)
                {
                    _useStaticFiles(app, options);
                    _useResponseCache(app, options);
                    _useSpa(app, options);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "");
            }
        }

        private static void _addDiscriminator(WebApplicationBuilder builder, Options options)
        {
            if (options.CacheResponse?.Enable == true)
            {
                // default generic discriminator
                builder.Services.TryAddSingleton(typeof(IDiscriminator), typeof(Discriminator));
                builder.Services.TryAddSingleton(typeof(IDiscriminator<>), typeof(Discriminator<>));
            }
        }
        private static void _useStaticFiles(WebApplication app, Options options)
        {
            if (options.StaticFilesPaths != null && options.StaticFilesPaths.Any())
            {
                foreach (var opt in options.StaticFilesPaths)
                {
                    if (opt != null)
                    {
                        var staticFileOptions = opt.GetStaticFileOptions(Path.Combine(env?.ContentRootPath ?? Directory.GetCurrentDirectory(), options.RootPath), logger);
                        app.UseSpaStaticFiles(staticFileOptions);
                    }
                }
            }
            else
                app.UseSpaStaticFiles();
        }

        private static void _useResponseCache(WebApplication app, Options options)
        {
            if (options.CacheResponse?.Enable == true)
            {
                app.UseMiddleware<ResponseCacheMiddleware>(options.CacheResponse);
            }
        }

        private static void _useSpa(WebApplication app, Options options)
        {
            app.UseSpa(_ =>
            {
                _.Options.DefaultPage = options.DefaultPage;
                _.Options.SourcePath = options.SourcePath;
                _.Options.StartupTimeout = TimeSpan.FromSeconds(options.StartupTimeoutInSeconds);

                if (!string.IsNullOrEmpty(options.SpaDevelopmentServer))
                    _.UseProxyToSpaDevelopmentServer(new Uri(options.SpaDevelopmentServer));
                if (!string.IsNullOrEmpty(options.CliServerScript))
                    _.UseReactDevelopmentServer(npmScript: options.CliServerScript);

            });
        }
    }

}

