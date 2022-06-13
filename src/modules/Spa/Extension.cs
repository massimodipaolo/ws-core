using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ws.Core.Extensions.Base;

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
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"{ex.Message} /n {ex.Source} /n {ex.StackTrace} /n {ex.InnerException}");
            }

        }

        public override void Execute(WebApplication app)
        {
            base.Execute(app);

            try
            {
                if (options != null)
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

                    if (options.CacheResponse?.Enable == true)
                    {
                        app.UseMiddleware<ResponseCacheMiddleware>(options.CacheResponse);
                    };

                    app.UseSpa(_ =>
                    {
                        _.Options.DefaultPage = options.DefaultPage;
                        _.Options.SourcePath = options.SourcePath;
                        _.Options.StartupTimeout = TimeSpan.FromSeconds(options.StartupTimeoutInSeconds);
                        //_.Options.PackageManagerCommand = "npm run dev";
                        
                        if (!string.IsNullOrEmpty(options.SpaDevelopmentServer))
                            _.UseProxyToSpaDevelopmentServer(new Uri(options.SpaDevelopmentServer));
                        //else 
                        if (!string.IsNullOrEmpty(options.CliServerScript))
                            _.UseReactDevelopmentServer(npmScript: options.CliServerScript);
                        
                    });
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"{ex.Message} /n {ex.Source} /n {ex.StackTrace} /n {ex.InnerException}");
            }
        }
    }

}

