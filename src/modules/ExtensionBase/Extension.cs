using ExtCore.Infrastructure.Actions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ws.Core.Extensions.Base
{
    public class Extension : ExtCore.Infrastructure.ExtensionBase, IConfigureAction, IConfigureServicesAction
    {
        private static IServiceProvider _serviceProvider;
        private static IServiceCollection _serviceCollection;
        protected IConfiguration _config => _serviceProvider?.GetService<IConfiguration>();
        protected IWebHostEnvironment _env => _serviceProvider?.GetService<IWebHostEnvironment>();
        protected ILogger _logger => _serviceProvider?.GetService<ILoggerFactory>()?.CreateLogger("Extension.Logger");

        public static void Init(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            if (null == _serviceCollection)            
                _serviceCollection = serviceCollection;   
            if (null == _serviceProvider)            
                _serviceProvider = serviceProvider;
        }

        protected string AssemblyName => GetType().GetTypeInfo().Assembly.GetName().Name;

        protected IEnumerable<Configuration.Assembly> Extensions => _config.GetSection($"{Configuration.SectionRoot}:Assemblies")?
                                           .Get<IDictionary<string, Extensions.Base.Configuration.Assembly>>()?
                                           .OrderBy(_ => _.Value.Priority)?
                                           .Select(_ => new Configuration.Assembly() { Name = _.Key, Priority = _.Value.Priority });
                                           //.Select((e,i) => new Configuration.Assembly() {Name = e.Key,Priority=i});

        protected Configuration.Assembly Assembly => Extensions?.Where(_ => _.Name == AssemblyName).FirstOrDefault();

        protected string ConfigSectionPathOptions => $"{Configuration.SectionRoot}:Assemblies:{AssemblyName}:Options";
        protected T GetOptions<T>() where T : class, new()
        {
            var obj = new T();
            if (Assembly != null)
                obj = _config?.GetSection(ConfigSectionPathOptions).Get<T>();            

            if (Extension.Option<T>.value == null)
                Extension.Option<T>.value = obj;

            return obj;
        }

        public virtual T ReloadOptions<T>() where T : class, new() 
        {            
            Func<object, string> serialize = t => Newtonsoft.Json.JsonConvert.SerializeObject(t);
            var _current = GetOptions<T>();
            if (serialize(Option<T>.value ?? new T()) == serialize(_current ?? new T())) {
                _logger.LogInformation($"{Name}: No changes, skip {DateTime.Now}");
                return null;   
            }
            else
            {                
                Option<T>.value = _current;
                _logger.LogInformation($"{Name}: Options reloaded {DateTime.Now}");
                return _current;
            }
        }

        public override string Name => AssemblyName;

        //IConfigureAction | IConfigureServicesAction
        public virtual int Priority => Assembly?.Priority ?? 0; // (Assembly?.Priority + 1) * 100 ?? 0;

        //IConfigureServicesAction
        public virtual void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider) {
        }

        //IConfigureAction
        public virtual void Execute(IApplicationBuilder applicationBuilder, IServiceProvider serviceProvider) {
            applicationBuilder.UseRouting();
        }

        public class Option<T>
        {
            public static T value { get; set; }
        }
        
    }
}
