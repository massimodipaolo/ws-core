using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using ExtCore.Infrastructure.Actions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;

namespace core.Extensions.Base
{
    public class Extension : ExtCore.Infrastructure.ExtensionBase, IConfigureAction, IConfigureServicesAction
    {        
        private static IServiceProvider _serviceProvider;
        private static IServiceCollection _serviceCollection;
        protected IConfiguration _config => _serviceProvider?.GetService<IConfiguration>();    
        protected IHostingEnvironment _env => _serviceProvider?.GetService<IHostingEnvironment>();
        protected ILogger _logger => _serviceProvider?.GetService<ILoggerFactory>()?.CreateLogger("Extension.Logger");        
        private static void Init(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            if (null == _serviceCollection)            
                _serviceCollection = serviceCollection;   
            if (null == _serviceProvider)            
                _serviceProvider = serviceProvider;   
        }
        protected string AssemblyName => GetType().GetTypeInfo().Assembly.GetName().Name;

        protected IEnumerable<Configuration.Assembly> Extensions => _config?.GetSection("Configuration:Assemblies").Get<IEnumerable<Configuration.Assembly>>();

        protected Configuration.Assembly Assembly => Extensions?.Select((e, i) => { e.Index = i; return e; }).Where(_ => _.Name == AssemblyName).FirstOrDefault();        

        protected T GetOptions<T>() where T : class, new()
        {
            var obj = new T();
            if (Assembly != null)
                obj = _config?.GetSection($"Configuration:Assemblies:{Assembly.Index}:Options").Get<T>();
                //obj = Assembly.Options as T;                

            return obj;
        }

        public override string Name => $"{AssemblyName} [{Priority}]";

        //IConfigureAction | IConfigureServicesAction
        public virtual int Priority => Assembly?.Index * 100 ?? 0;

        //IConfigureServicesAction
        public virtual void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider) {
            Init(serviceCollection,serviceProvider);
        }

        //IConfigureAction
        public virtual void Execute(IApplicationBuilder applicationBuilder, IServiceProvider serviceProvider) {            
        }

        public virtual void Reload(IApplicationBuilder applicationBuilder)
        {
            _logger.LogInformation($"{Name}: Option reloading {DateTime.Now}");
        }

    }
}
