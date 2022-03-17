using ExtCore.Infrastructure;
using ExtCore.Infrastructure.Actions;
using ExtCore.WebApplication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ws.Core.Extensions
{
    public static class ServiceCollection
    {
        public static void AddExtCoreWithInjectors(this IServiceCollection services, string extensionsPath, bool includingSubpaths = false)
        {
            DiscoverAssemblies(new DefaultAssemblyProvider(services.BuildServiceProvider()), extensionsPath, includingSubpaths);
            IServiceProvider serviceProvider = services.BuildServiceProvider();
            ILogger logger = serviceProvider.GetService<ILoggerFactory>()!.CreateLogger("ExtCore.WebApplication");

            foreach (IConfigureServicesAction item in from a in ExtensionManager.GetInstances<IConfigureServicesAction>()
                                                      .UnionInjector()
                                                      orderby a.Priority
                                                      select a)
            {
                logger.LogInformation("Executing ConfigureServices action '{0}'", item.GetType().FullName);
                item.Execute(services, serviceProvider);
                serviceProvider = services.BuildServiceProvider();
            }
        }

        private static void DiscoverAssemblies(IAssemblyProvider assemblyProvider, string extensionsPath, bool includingSubpaths)
        {
            ExtensionManager.SetAssemblies(assemblyProvider.GetAssemblies(extensionsPath, includingSubpaths));
        }
    }
}
