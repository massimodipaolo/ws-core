using ExtCore.Infrastructure;
using ExtCore.Infrastructure.Actions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ws.Core.Extensions
{
    public static class ApplicationBuilder
    {
        public static void UseExtCoreWithInjectors(this IApplicationBuilder applicationBuilder)
        {
            ILogger logger = applicationBuilder.ApplicationServices.GetService<ILoggerFactory>()!.CreateLogger("ExtCore.WebApplication");
            foreach (IConfigureAction item in from a in ExtensionManager.GetInstances<IConfigureAction>()
                                              .UnionInjector()
                                              orderby a.Priority
                                              select a)
            {
                logger.LogInformation("Executing Configure action '{0}'", item.GetType().FullName);
                item.Execute(applicationBuilder, applicationBuilder.ApplicationServices);
            }
        }
    }
}
