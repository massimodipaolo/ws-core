using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Ws.Core.Extensions.Base;

public class Extension : ExtCore.Infrastructure.IExtension, ExtCore.Infrastructure.Actions.IConfigureBuilder, ExtCore.Infrastructure.Actions.IConfigureApp
{
    private static IServiceProvider? _serviceProvider;
    private static IServiceCollection? _serviceCollection;
    protected static IConfiguration? config => _serviceProvider?.GetService<IConfiguration>();
    protected static IWebHostEnvironment? env => _serviceProvider?.GetService<IWebHostEnvironment>();
    protected static ILogger? logger => _serviceProvider?.GetService<ILoggerFactory>()?.CreateLogger("Extension.Logger");

    public static void Init(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
    {
        if (null == Extension._serviceCollection)
            Extension._serviceCollection = serviceCollection;
        if (null == Extension._serviceProvider)
            Extension._serviceProvider = serviceProvider;
    }

    protected string? AssemblyName => GetType()?.GetTypeInfo().Assembly.GetName().Name;

    protected static IEnumerable<Configuration.Assembly>? Extensions => config?.GetSection($"{Configuration.SectionRoot}:Assemblies")?
                                       .Get<IDictionary<string, Extensions.Base.Configuration.Assembly>>()?
                                       .OrderBy(_ => _.Value.Priority)?
                                       .Select(_ => new Configuration.Assembly() { Name = _.Key, Priority = _.Value.Priority });

    protected Configuration.Assembly? Assembly => Extensions?.FirstOrDefault(_ => _.Name == AssemblyName);

    protected string ConfigSectionPathOptions => $"{Configuration.SectionRoot}:Assemblies:{AssemblyName}:Options";
    protected T GetOptions<T>() where T : class, IOptions, new()
    {
        var obj = new T();
        if (Assembly != null)
            obj = config?.GetSection(ConfigSectionPathOptions).Get<T>() ?? obj;

        Extension.Option<T>.Value ??= obj;

        return obj;
    }

    protected (string prefix, string tag) GetApiRoute([System.Runtime.CompilerServices.CallerMemberName] string caller = "")
    {
        var _name = Assembly?.Name ?? caller?.ToLower();
        var tag = $"{nameof(Extensions)}-{_name}".ToLower();
        var prefix = $"{nameof(Extensions)}/{_name}".ToLower();
        return (prefix, tag);
    }

    public virtual string Name => AssemblyName ?? GetType().FullName ?? string.Empty;
    public virtual int Priority => Assembly?.Priority ?? 0;


    //IConfigureBuilder
    public virtual void Add(WebApplicationBuilder builder, IServiceProvider? serviceProvider = null)
    {
    }

    //IConfigureApp
    public virtual void Use(WebApplication app)
    {
    }

    public class Option<T>
    {
        protected Option() { }
        public static T? Value { get; set; }
    }

}