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
        /// <summary>
        /// Executes the Configure actions from all the extensions. It must be called inside the Program file
        /// after the AddExtCore extension.
        /// </summary>
        /// <param name="app">
        /// The Microsoft.AspNetCore.Builder.WebApplication passed to the Configure method of the web application's Startup class.
        /// </param>        
        public static void UseExtCore(this WebApplication app)
        {
            ILogger logger = app.Services.GetService<ILoggerFactory>()!.CreateLogger($"{nameof(ExtCore)}.{nameof(ExtCore.Application)}");
            foreach (IConfigureApp item in from a in ExtensionManager.GetInstances<IConfigureApp>()
                                              .UnionInjector()
                                              orderby a.Priority
                                              select a)
            {
                logger.LogInformation($"Executing Configure action '{item.GetType().FullName}'");
                item.Execute(app);
            }
        }
    }
}
