using ExtCore.Application;
using ExtCore.Infrastructure;
using ExtCore.Infrastructure.Actions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Ws.Core.Extensions;

public static class ServiceCollection
{
    public static void AddExtCore(this WebApplicationBuilder builder, string? extensionsPath = null, bool includingSubpaths = false)
    {
        // init
        IServiceProvider serviceProvider = builder.Services.BuildServiceProvider();
        DiscoverAssemblies(new DefaultAssemblyProvider(serviceProvider), extensionsPath, includingSubpaths);
        ILogger logger = serviceProvider.GetService<ILoggerFactory>()!.CreateLogger($"{nameof(ExtCore)}.{nameof(ExtCore.Application)}");

        foreach (IConfigureBuilder item in from a in ExtensionManager.GetInstances<IConfigureBuilder>()
                                                  .UnionInjector()
                                           orderby a.Priority
                                           select a)
        {
            logger.LogInformation("Executing ConfigureServices action '{type}'", item.GetType().FullName);
            item.Add(builder, serviceProvider);
            // new container
            serviceProvider = builder.Services.BuildServiceProvider();
        }
    }

    private static void DiscoverAssemblies(IAssemblyProvider assemblyProvider, string? extensionsPath = null, bool includingSubpaths = false)
    {
        ExtensionManager.SetAssemblies(assemblyProvider.GetAssemblies(extensionsPath, includingSubpaths));
    }
}
