using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;

namespace core.Extension
{
    public class ExtensionBase : ExtCore.Infrastructure.ExtensionBase
    {           
        protected string AssemblyName => GetType().GetTypeInfo().Assembly.GetName().Name;

        protected IEnumerable<Configuration.Assembly> Extensions
        {
            get
            {   
                return configurationRoot?.GetSection("Extensions").Get<IEnumerable<Configuration.Assembly>>();             
            }
        }

        protected Configuration.Assembly Assembly
        {
            get
            {
                return Extensions.Select((e, i) => { e.Index = i; return e; }).Where(_ => _.Name == AssemblyName).FirstOrDefault();
            }
        }

        protected int Priority => Assembly?.Index * 100 ?? 0;

        protected T GetOptions<T>() where T: class, new()
        {            
            var obj = new T();            
            if (Assembly != null)            
                obj = configurationRoot.GetSection($"Extensions:{Assembly.Index}:Options").Get<T>();            
            return obj;
        }
        public override string Name => $"{AssemblyName} [{Assembly?.Index}]";

        protected IHostingEnvironment Environment => serviceProvider.GetService<IHostingEnvironment>();

    }
}
