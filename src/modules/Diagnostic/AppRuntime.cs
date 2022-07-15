using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Ws.Core.Extensions.Data.Cache;

namespace Ws.Core.Extensions.Diagnostic;

public class AppRuntime : AppRuntime<Ws.Core.AppConfig> { }
public class AppRuntime<TConfig> where TConfig : class, IAppConfiguration, new()
{
    public AppRuntime() { }
    public InfoData Info { get; set; }
    public dynamic? Cache { get; set; }
    public dynamic? Environment { get; set; }
    public dynamic? Config { get; set; }
    public IEnumerable<string>? Assemblies { get; set; }
    public dynamic? Extensions { get; set; }
    public dynamic? Services { get; set; }
    public struct InfoData
    {
        public DateTime Uptime { get; set; }
        public IEnumerable<KeyValuePair<string, string>>? ServerFeatures { get; set; }
        public ComputerData Computer { get; set; }
        public IHeaderDictionary? Headers { get; set; }
        public string? RemoteIpAddress { get; set; }
        public struct ComputerData
        {
            public string MachineName { get; set; }
            public OperatingSystem Os { get; set; }
            public int ProcessorCount { get; set; }
            public string SystemDirectory { get; set; }
            public dynamic? Drivers { get; set; }
            public string User { get; set; }
            public dynamic? Process { get; set; }
        }
    }
    /// <summary>
    /// Return system and application diagnostic informations, about the environment, app configuration, extentions injected, service implementations
    /// </summary>
    /// <returns></returns>
    public static AppRuntime Get(
        ICache cache,
        IConfiguration config,
        IWebHostEnvironment env,
        IHttpContextAccessor ctx
        )
    {
        var mainModule = System.Diagnostics.Process.GetCurrentProcess().MainModule;
        var runtime = new AppRuntime()
        {
            Info = new()
            {
                Uptime = Startup.Uptime,
                ServerFeatures = AppInfo.App?.ServerFeatures?.Select(_ => new KeyValuePair<string, string>(_.Key.ToString(), _.Value?.ToString() ?? "")),
                Computer = new()
                {
                    MachineName = System.Environment.MachineName,
                    Os = System.Environment.OSVersion,
                    ProcessorCount = System.Environment.ProcessorCount,
                    Drivers = DriveInfo.GetDrives()?.Where(_ => _.IsReady)?.Select(_ => new { _.Name, TotalSizeGb = _.TotalSize / 1024.0 / 1024.0 / 1024.0, AvailableFreeSpaceGb = _.AvailableFreeSpace / 1024.0 / 1024.0 / 1024.0, _.VolumeLabel, _.DriveFormat, DriveType = _.DriveType.ToString() }),
                    SystemDirectory = System.Environment.SystemDirectory,
                    User = $"{System.Environment.UserDomainName}\\{System.Environment.UserName}",
                    Process = new { mainModule?.FileName, mainModule?.FileVersionInfo, mainModule?.ModuleMemorySize, mainModule?.ModuleName }
                },
                Headers = ctx.HttpContext?.Request?.Headers,
                RemoteIpAddress = ctx.HttpContext?.Connection?.RemoteIpAddress?.ToString()
            },
            Cache = cache.Keys != null ? cache.Keys.Count() : 0,
            Environment = new { env.ApplicationName, env.EnvironmentName, env.ContentRootPath, env.WebRootPath },
            Config = new
            {
                detail = config?.AsEnumerable()
                .Select(conf => new
                {
                    conf.Key,
                    Value = new string[] { "connectionstring", "username", "password", "pwd", "secret", "apikey" }.Any(s => conf.Key.ToLower().Contains(s))
                        ? new string('*', 8) : conf.Value
                })
                .OrderBy(conf => conf.Key)
                .ToDictionary(conf => conf.Key, conf => conf.Value)
            },
            Assemblies = new ExtCore.Application.DefaultAssemblyProvider(AppInfo.ServiceProvider)
                .GetAssemblies(Startup.ExtCoreExtensionsPath(config, env), includingSubpaths: true)
                .Select(_ => _.FullName ?? ""),
            Extensions = ExtCore.Infrastructure.ExtensionManager.GetInstances<ExtCore.Infrastructure.Actions.IConfigureBuilder>()
                .UnionInjector()
                .Where(_ => _ is Ws.Core.Extensions.Base.Extension)
                .Select(_ => new { ((Ws.Core.Extensions.Base.Extension)_)?.Name, _?.Priority })
                .OrderBy(_ => _.Priority),
            Services = AppInfo.Services?.Select(_ => new
            {
                Type = _.ServiceType?.FullName,
                ImplementationType = _.ImplementationType?.FullName,
                Lifetime = _.Lifetime.ToString()
            })
        };
        return runtime;
    }

    /// <summary>
    /// Perform an application lifetime stop
    /// </summary>
    /// <returns></returns>        
    public static async Task<object> Stop(IHostApplicationLifetime applicationLifetime)
    {
        await Task.Run(() => applicationLifetime.StopApplication());
        return Task.FromResult(new { Startup<TConfig>.Uptime, status = "Stopping" });
    }
}
