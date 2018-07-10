using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace core.Extensions.Spa
{
    public class Extension : Base.Extension
    {
        private Options _options => GetOptions<Options>();

        public override void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            base.Execute(serviceCollection, serviceProvider);

            serviceCollection.AddSpaStaticFiles(_ =>
            {
                _.RootPath = _options.RootPath;
            });
            if (_options.Predendering != null)
                serviceCollection.AddSpaPrerenderer();
        }

        public override void Execute(IApplicationBuilder applicationBuilder, IServiceProvider serviceProvider)
        {
            base.Execute(applicationBuilder, serviceProvider);

            applicationBuilder.UseSpaStaticFiles();
            applicationBuilder.UseSpa(_ =>
            {
                _.Options.DefaultPage = _options.DefaultPage;
                _.Options.SourcePath = _options.SourcePath;
                _.Options.StartupTimeout = TimeSpan.FromSeconds(_options.StartupTimeoutInSeconds);

                if (!string.IsNullOrEmpty(_options.SpaDevelopmentServer))
                    _.UseProxyToSpaDevelopmentServer(new Uri(_options.SpaDevelopmentServer));
                else if (!string.IsNullOrEmpty(_options.CliServerScript))
                    _.UseAngularCliServer(npmScript: _options.CliServerScript);

                if (_options.Predendering != null)
                {
                    _.UseSpaPrerendering(conf =>
                    {
                        if (!string.IsNullOrEmpty(_options.Predendering.BootModuleBuilderScript))
                            conf.BootModuleBuilder = new AngularCliBuilder(npmScript: _options.Predendering.BootModuleBuilderScript);
                        conf.BootModulePath = _options.Predendering.BootModulePath;
                        conf.ExcludeUrls = _options.Predendering.ExcludeUrls;
                        if (_options.Predendering.ContextData.Any())
                        {
                            conf.SupplyData = (ctx, data) =>
                            {
                                foreach (var p in _options.Predendering.ContextData)
                                {
                                    switch (p)
                                    {
                                        case "features":
                                            data[p] = ctx.Features;
                                            break;
                                        case "items":
                                            data[p] = ctx.Items;
                                            break;
                                        case "request":
                                            data[p] = ctx.Request;
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

            });
        }
    }

}

