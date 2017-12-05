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

            if (Option<T>.value == null)
                Option<T>.value = obj;

            return obj;
        }

        public virtual void ReloadOptions<T>(ConfigurationChangeContext ctx) where T : class, new() 
        {            
            Func<object, string> serialize = t => Newtonsoft.Json.JsonConvert.SerializeObject(t);
            var _current = GetOptions<T>();
            if (serialize(Option<T>.value ?? new T()) == serialize(_current ?? new T())) 
                _logger.LogInformation($"{Name}: No changes, skip {DateTime.Now}");
            else
            {                
                Execute(ctx.App, _serviceProvider);
                Execute(_serviceCollection, _serviceProvider);
                Option<T>.value = _current;
                _logger.LogInformation($"{Name}: Options reloaded {DateTime.Now}");
            }
        }

        public override string Name => AssemblyName;

        //IConfigureAction | IConfigureServicesAction
        public virtual int Priority => (Assembly?.Index + 1) * 100 ?? 0;

        //IConfigureServicesAction
        public virtual void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider) {
            Init(serviceCollection,serviceProvider);
        }

        //IConfigureAction
        public virtual void Execute(IApplicationBuilder applicationBuilder, IServiceProvider serviceProvider) {            
        }

        public class Option<T>
        {
            public static object value { get; set; }
        }
        
    }
}
