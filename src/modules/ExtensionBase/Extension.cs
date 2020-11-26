using ExtCore.Infrastructure.Actions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ws.Core.Extensions.Base
{
    public class Extension : ExtCore.Infrastructure.ExtensionBase, IConfigureAction, IConfigureServicesAction
    {
        private static IServiceProvider serviceProvider;
        private static IServiceCollection serviceCollection;
        protected static IConfiguration config => serviceProvider?.GetService<IConfiguration>();
        protected static IWebHostEnvironment env => serviceProvider?.GetService<IWebHostEnvironment>();
        protected static ILogger logger => serviceProvider?.GetService<ILoggerFactory>()?.CreateLogger("Extension.Logger");

        public static void Init(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            if (null == Extension.serviceCollection)
                Extension.serviceCollection = serviceCollection;   
            if (null == Extension.serviceProvider)
                Extension.serviceProvider = serviceProvider;
        }

        protected string AssemblyName => GetType().GetTypeInfo().Assembly.GetName().Name;

        protected static IEnumerable<Configuration.Assembly> Extensions => config.GetSection($"{Configuration.SectionRoot}:Assemblies")?
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
                obj = config?.GetSection(ConfigSectionPathOptions).Get<T>();            

            if (Extension.Option<T>.Value == null)
                Extension.Option<T>.Value = obj;

            return obj;
        }

        public virtual T ReloadOptions<T>() where T : class, new() 
        {
            static string serialize(object t) => Newtonsoft.Json.JsonConvert.SerializeObject(t);
            var _current = GetOptions<T>();
            if (serialize(Option<T>.Value ?? new T()) == serialize(_current ?? new T())) {
                logger.LogInformation($"{Name}: No changes, skip {DateTime.Now}");
                return null;   
            }
            else
            {                
                Option<T>.Value = _current;
                logger.LogInformation($"{Name}: Options reloaded {DateTime.Now}");
                return _current;
            }
        }

        public override string Name => AssemblyName;

        //IConfigureAction | IConfigureServicesAction
        public virtual int Priority => Assembly?.Priority ?? 0; // (Assembly?.Priority + 1) * 100 ?? 0;

        //IConfigureServicesAction
        public virtual void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider) {
            // default generic discriminator
            serviceCollection.TryAddSingleton(typeof(IDiscriminator), typeof(Discriminator));
            serviceCollection.TryAddSingleton(typeof(IDiscriminator<>), typeof(Discriminator<>));
        }

        //IConfigureAction
        public virtual void Execute(IApplicationBuilder applicationBuilder, IServiceProvider serviceProvider) {
        }

        public class Option<T>
        {
            public static T Value { get; set; }
        }
        
    }
}
