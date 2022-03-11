using ExtCore.Infrastructure.Actions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace xCore.Extensions
{
    public class PreConfigureServicesInject : ExtCore.Infrastructure.ExtensionBase, IConfigureAction, IConfigureServicesAction
    {
        public override string Name => typeof(PreConfigureServicesInject).Name;

        public virtual int Priority => int.MinValue;

        //IConfigureServicesAction
        public virtual void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            //serviceCollection.TryAdd(new ServiceDescriptor(typeof(object), typeof(object), ServiceLifetime.Transient));
            //serviceCollection.Add(new ServiceDescriptor(typeof(object), typeof(object), ServiceLifetime.Transient));
        }

        //IConfigureAction
        public virtual void Execute(IApplicationBuilder applicationBuilder, IServiceProvider serviceProvider)
        {
        }
    }

    public class PostConfigureServicesInject : ExtCore.Infrastructure.ExtensionBase, IConfigureAction, IConfigureServicesAction
    {
        public override string Name => typeof(PostConfigureServicesInject).Name;

        public virtual int Priority => int.MaxValue;

        //IConfigureServicesAction
        public virtual void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
        }

        //IConfigureAction
        public virtual void Execute(IApplicationBuilder applicationBuilder, IServiceProvider serviceProvider)
        {
        }
    }
}
