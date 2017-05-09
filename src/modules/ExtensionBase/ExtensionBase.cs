using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace core.Extension
{
    public class ExtensionBase : ExtCore.Infrastructure.ExtensionBase
    {        
        protected string AssemblyName => GetType().GetTypeInfo().Assembly.GetName().Name;

        protected IEnumerable<Configuration.Options.Extension> AllExtensions
        {
            get
            {                
                return configurationRoot?.GetSection("Extensions").Get<IEnumerable<Configuration.Options.Extension>>();             
            }
        }

        protected Configuration.Options.Extension Configuration
        {
            get
            {
                return AllExtensions.Select((e, i) => { e.Index = i; return e; }).Where(_ => _.Name == AssemblyName).FirstOrDefault();
            }
        }

        protected int Priority => Configuration?.Index * 100 ?? 0;

        protected T GetOptions<T>() where T: class, new()
        {
            var obj = new T();            
            if (Configuration != null)            
                obj = configurationRoot.GetSection($"Extensions:{Configuration.Index}:Options").Get<T>();            
            return obj;
        }
        public override string Name => $"{AssemblyName} [{Configuration?.Index}]";        
    }
}
