using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;

namespace Ws.Core.Extensions.Spa
{
    public class Extension : Base.Extension
    {
        private Options _options => GetOptions<Options>();


        public override void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            base.Execute(serviceCollection, serviceProvider);

            try
            {
                if (_options != null)
                {
                    serviceCollection.AddSpaStaticFiles(_ =>
                    {
                        _.RootPath = _options.RootPath;
                    });
                    if (_options.Prerendering != null)
                        serviceCollection.AddSpaPrerenderer();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message} /n {ex.Source} /n {ex.StackTrace} /n {ex.InnerException}");
            }

        }

        public override void Execute(IApplicationBuilder applicationBuilder, IServiceProvider serviceProvider)
        {
            base.Execute(applicationBuilder, serviceProvider);

            try
            {
                if (_options != null)
                {
                    if (_options.StaticFilesPaths != null && _options.StaticFilesPaths.Any())
                    {
                        foreach(var opt in _options.StaticFilesPaths)
                        {
                            if (opt != null)
                            {
                                var staticFileOptions = opt.GetStaticFileOptions(Path.Combine(_env?.ContentRootPath ?? Directory.GetCurrentDirectory(), _options.RootPath),_env,_logger);
                                applicationBuilder.UseSpaStaticFiles(staticFileOptions);
                            }
                        }
                    }
                    else
                        applicationBuilder.UseSpaStaticFiles();

                    if (_options.Prerendering != null && _options.Prerendering.Enable && _options.Prerendering.CacheResponse != null && _options.Prerendering.CacheResponse.Enable)
                    {
                        applicationBuilder.UseMiddleware<ResponseCacheMiddleware>(_options.Prerendering.CacheResponse);
                    };

                    applicationBuilder.UseSpa(_ =>
                    {
                        _.Options.DefaultPage = _options.DefaultPage;
                        _.Options.SourcePath = _options.SourcePath;
                        _.Options.StartupTimeout = TimeSpan.FromSeconds(_options.StartupTimeoutInSeconds);

                        if (_options.Prerendering != null && _options.Prerendering.Enable)
                        {
                            _.UseSpaPrerendering(conf =>
                            {
                                if (!string.IsNullOrEmpty(_options.Prerendering.BootModuleBuilderScript))
                                    conf.BootModuleBuilder = new AngularCliBuilder(npmScript: _options.Prerendering.BootModuleBuilderScript);
                                conf.BootModulePath = _options.Prerendering.BootModulePath;
                                conf.ExcludeUrls = _options.Prerendering.ExcludeUrls;
                                if (_options.Prerendering.ContextData != null && _options.Prerendering.ContextData.Any())
                                {
                                    conf.SupplyData = (ctx, data) =>
                                    {
                                        foreach (var p in _options.Prerendering.ContextData)
                                        {
                                            switch (p)
                                            {
                                                case "features":
                                                    data[p] = ctx.Features;
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

                        if (!string.IsNullOrEmpty(_options.SpaDevelopmentServer))
                            _.UseProxyToSpaDevelopmentServer(new Uri(_options.SpaDevelopmentServer));
                        else if (!string.IsNullOrEmpty(_options.CliServerScript))
                            _.UseAngularCliServer(npmScript: _options.CliServerScript);

                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message} /n {ex.Source} /n {ex.StackTrace} /n {ex.InnerException}");
            }
        }
    }

}

