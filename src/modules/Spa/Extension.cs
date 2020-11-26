using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SpaServices.AngularCli;
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

        public override void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            base.Execute(serviceCollection, serviceProvider);

            try
            {
                if (options != null)
                {
                    serviceCollection.AddSpaStaticFiles(_ =>
                    {
                        _.RootPath = options.RootPath;
                    });
                    if (options.Prerendering != null && options.Prerendering.Enable)
                        serviceCollection.AddSpaPrerenderer();
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"{ex.Message} /n {ex.Source} /n {ex.StackTrace} /n {ex.InnerException}");
            }

        }

        public override void Execute(IApplicationBuilder applicationBuilder, IServiceProvider serviceProvider)
        {
            base.Execute(applicationBuilder, serviceProvider);

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
                                applicationBuilder.UseSpaStaticFiles(staticFileOptions);
                            }
                        }
                    }
                    else
                        applicationBuilder.UseSpaStaticFiles();

                    if (options.Prerendering != null && options.Prerendering.Enable && options.Prerendering.CacheResponse != null && options.Prerendering.CacheResponse.Enable)
                    {
                        applicationBuilder.UseMiddleware<ResponseCacheMiddleware>(options.Prerendering.CacheResponse);
                    };

                    applicationBuilder.UseSpa(_ =>
                    {
                        _.Options.DefaultPage = options.DefaultPage;
                        _.Options.SourcePath = options.SourcePath;
                        _.Options.StartupTimeout = TimeSpan.FromSeconds(options.StartupTimeoutInSeconds);

                        if (options.Prerendering != null && options.Prerendering.Enable)
                        {
                            _.UseSpaPrerendering(conf =>
                            {
                                if (!string.IsNullOrEmpty(options.Prerendering.BootModuleBuilderScript))
                                    conf.BootModuleBuilder = new AngularCliBuilder(npmScript: options.Prerendering.BootModuleBuilderScript);
                                conf.BootModulePath = options.Prerendering.BootModulePath;
                                conf.ExcludeUrls = options.Prerendering.ExcludeUrls;
                                if (options.Prerendering.ContextData != null && options.Prerendering.ContextData.Any())
                                {
                                    conf.SupplyData = (ctx, data) =>
                                    {
                                        foreach (var p in options.Prerendering.ContextData)
                                        {
                                            switch (p)
                                            {
                                                case "cookies":
                                                    data[p] = ctx.Request?.Cookies;
                                                    break;
                                                case "features":
                                                    data[p] = ctx.Features;
                                                    break;
                                                case "headers":
                                                    data[p] = ctx.Request?.Headers;
                                                    break;
                                                case "items":
                                                    data[p] = ctx.Items;
                                                    break;
                                                case "session":
                                                    data[p] = ctx.Session;
                                                    break;
                                                case "user":
                                                    data[p] = ctx.User;
                                                    break;
                                                case "webSockets":
                                                    data[p] = ctx.WebSockets;
                                                    break;
                                            }
                                        }
                                    };
                                }
                            });
                        }

                        if (!string.IsNullOrEmpty(options.SpaDevelopmentServer))
                            _.UseProxyToSpaDevelopmentServer(new Uri(options.SpaDevelopmentServer));
                        else if (!string.IsNullOrEmpty(options.CliServerScript))
                            _.UseAngularCliServer(npmScript: options.CliServerScript);

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

