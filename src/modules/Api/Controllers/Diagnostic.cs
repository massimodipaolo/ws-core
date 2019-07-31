using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ws.Core.Extensions.Data.Cache;

namespace Ws.Core.Extensions.Api.Controllers
{
    public class DiagnosticController<TConfig> : BaseController where TConfig: class, Ws.Core.IAppConfiguration, new()
    {
        private ICache _cache;
        private IConfiguration _config;
        private IHostingEnvironment _env;
        private IOptionsMonitor<TConfig> _appConfigMonitor;
        private IOptionsMonitor<Ws.Core.Extensions.Base.Configuration> _extConfigMonitor;
        private IApplicationLifetime _applicationLifetime;
        public DiagnosticController(
            ICache cache,
            IConfiguration config,
            IHostingEnvironment env,
            IOptionsMonitor<TConfig> appConfigMonitor,
            IOptionsMonitor<Ws.Core.Extensions.Base.Configuration> extConfigMonitor,
            IApplicationLifetime applicationLifetime,
            IHttpContextAccessor ctx
            ) : base(ctx)
        {
            _cache = cache;
            _config = config;
            _env = env;
            _appConfigMonitor = appConfigMonitor;
            _extConfigMonitor = extConfigMonitor;
            _applicationLifetime = applicationLifetime;
        }

        /// <summary>
        /// Return system and application diagnostic informations, about the environment, app configuration, extentions injected, service implementations
        /// </summary>
        /// <returns></returns>
        public virtual IActionResult Get()
        {
            var diag = new
            {
                info = new
                {
                    uptime=Startup<TConfig>._uptime,
                    app = AppInfo<TConfig>.App,
                    env = new
                    {
                        machine = Environment.MachineName,
                        os = Environment.OSVersion,
                        processorCount = Environment.ProcessorCount,
                        systemDirectory = Environment.SystemDirectory,
                        user = $"{Environment.UserDomainName}\\{Environment.UserName}",
                        process = System.Diagnostics.Process.GetCurrentProcess().MainModule
                    },
                    remoteIpAddress = _ctx.HttpContext?.Connection?.RemoteIpAddress.ToString()
                },
                cache = _cache.Keys != null ? _cache.Keys.Count() : 0,
                environment = new { _env.ApplicationName, _env.EnvironmentName, _env.ContentRootPath, _env.WebRootPath },
                config = new
                {
                    //builder = _config,
                    detail = _config.AsEnumerable()
                        .Select(conf => new {
                            conf.Key,
                            Value = new string[] { "connectionstring", "username", "password", "pwd", "secret", "apikey" }.Any(s => conf.Key.ToLower().Contains(s))
                                ?
                                    new string('*', 8)
                                :
                                conf.Value
                        })
                        .OrderBy(conf => conf.Key)
                        .ToDictionary(conf => conf.Key, conf => conf.Value)
                },
                extensions = _extConfigMonitor.CurrentValue.Assemblies
                    .Select(_ => new { Name = _.Key, _.Value.Priority })
                    .Union(
                        ExtCore.Infrastructure.ExtensionManager.GetInstances<ExtCore.Infrastructure.Actions.IConfigureServicesAction>()
                        .Where(_ => _ is ExtCore.Infrastructure.ExtensionBase)
                        .Select(_ => new { (_ as ExtCore.Infrastructure.ExtensionBase).Name, _.Priority })
                    ).
                    OrderBy(_ => _.Priority),
                //extensions = string.Join(" | ", ExtCore.Infrastructure.ExtensionManager.GetInstances<core.Extensions.Base.Extension>().OrderBy(ext => ext.Priority).Select(ext => $"{ext.Name} [{ext.Priority}]")),
                services = AppInfo<TConfig>.Services?.Select(_ => new { Type = _.ServiceType, _.ImplementationType, Lifetime = _.Lifetime.ToString() })
            };
            return Ok(diag);
        }

        /// <summary>
        /// Perform an application lifetime stop
        /// </summary>
        /// <returns></returns>        
        public virtual async Task<IActionResult> Stop()
        {
            await System.Threading.Tasks.Task.Run(() => _applicationLifetime.StopApplication());
            return Ok(new { uptime = Startup<TConfig>._uptime, status = "Stopping" });
        }

    }
}
