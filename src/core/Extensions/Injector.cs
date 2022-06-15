using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Ws.Core.Extensions.Base;

namespace Ws.Core.Extensions
{
    public class Injector : Base.Extension
    {

        private static IEnumerable<Injector> _list;
        private static readonly Util.Locker _mutexList = new();
        private static string _configSectionPathInjectors => $"{Configuration.SectionRoot}:Injectors";
        private static IEnumerable<Configuration.Injector> _getConfigInjectors()
        => config?.GetSection(_configSectionPathInjectors)?
            .Get<IEnumerable<Configuration.Injector>>()?
            .Select((obj, idx) =>
            {
                if (string.IsNullOrEmpty(obj.Name))
                    obj.Name = $"{nameof(Injector)}-{idx}";
                return obj;
            }) ?? Array.Empty<Configuration.Injector>();

        private Configuration.Injector _getOptions()
        => _getConfigInjectors()?.FirstOrDefault(_ => _.Name == this.Name);

        public static IEnumerable<Ws.Core.Extensions.Injector> List()
        {
            if (_list == null)
                using (_mutexList.Lock())
                    if (_list == null)
                    {
                        _list = new List<Injector>();
                        foreach (var config in _getConfigInjectors())
                        {
                            Ws.Core.Extensions.Injector item = (Injector)Activator.CreateInstance(typeof(Ws.Core.Extensions.Injector), config);
                            ((List<Injector>)_list).Add(item);
                        }
                    }
            return _list;
        }
        private Configuration.Injector _options { get; set; }
        public Injector()
        {
            _options = _getOptions();
        }
        public Injector(Configuration.Injector config)
        {
            _options = config;
        }        
        public override string Name => _options?.Name ?? "";
        public override int Priority => _options?.Priority ?? 0;
        public override void Execute(WebApplicationBuilder builder, IServiceProvider serviceProvider = null)
        {
            _addServices(builder, _options?.Services);
            _addDecorators(builder, _options?.Decorators);
        }
        public override void Execute(WebApplication app)
        {
            // https://docs.microsoft.com/it-it/aspnet/core/fundamentals/middleware/?view=aspnetcore-6.0
            // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/write?view=aspnetcore-6.0
            if (_options?.Middlewares != null)
                foreach (var middleware in _options.Middlewares)
                {
                    Type type = _getType(middleware.Type);
                    if (type != null)
                    {
                        if (middleware.Map != null && !string.IsNullOrEmpty(middleware.Map.PathMatch))
                            app.Map(middleware.Map.PathMatch, middleware.Map.PreserveMatchedPathSegment, _ => _.UseMiddleware(type));
                        else
                            app.UseMiddleware(type);
                    }
                }
        }

        static Type _getType(string s) => !string.IsNullOrEmpty(s) ? (Type.GetType(s, throwOnError: false) ?? Ws.Core.Extensions.Base.Util.GetType(s)) : null;
        private void _addServices(WebApplicationBuilder builder, Configuration.Injector.ServiceOption[] services = null)
        {
            if (services?.Any() == true)
                foreach (var service in _options.Services)
                {
                    Type serviceType = _getType(service.ServiceType);
                    Type implementationType = _getType(service.ImplementationType);
                    if (serviceType != null && implementationType != null && serviceType.IsAssignableFrom(implementationType))
                    {
                        var descriptor = new ServiceDescriptor(serviceType, implementationType, service.Lifetime);
                        if (service.OverrideIfAlreadyRegistered)
                            builder.Services.Add(descriptor);
                        else
                            builder.Services.TryAdd(descriptor);
                    }
                }
        }
        private void _addDecorators(WebApplicationBuilder builder, Configuration.Injector.DecoratorOption[] decorators = null)
        {
            if (decorators?.Any() == true)
                foreach (var decorator in _options.Decorators)
                {
                    Type serviceType = _getType(decorator.ServiceType);
                    Type implementationType = _getType(decorator.ImplementationType);
                    if (serviceType != null && implementationType != null && serviceType.IsAssignableFrom(implementationType))
                        builder.Services.TryDecorate(serviceType, implementationType);
                }
        }
    }
}
