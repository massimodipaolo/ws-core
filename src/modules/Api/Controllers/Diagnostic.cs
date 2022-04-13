using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ws.Core.Extensions.Data.Cache;

namespace Ws.Core.Extensions.Api.Controllers
{
    public class DiagnosticController<TConfig> : BaseController where TConfig : class, Ws.Core.IAppConfiguration, new()
    {
        private readonly ICache _cache;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;
        private readonly IHostApplicationLifetime _applicationLifetime;
        public DiagnosticController(
            ICache cache,
            IConfiguration config,
            IWebHostEnvironment env,
            IHostApplicationLifetime applicationLifetime,
            IHttpContextAccessor ctx
            ) : base(ctx)
        {
            _cache = cache;
            _config = config;
            _env = env;
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
                    Startup<TConfig>.Uptime,
                    app = AppInfo<TConfig>.App?.ServerFeatures,
                    env = new
                    {
                        machine = Environment.MachineName,
                        os = Environment.OSVersion,
                        processorCount = Environment.ProcessorCount,
                        drivers = DriveInfo.GetDrives()?.Where(_ => _.IsReady)?.Select(_ => new { _.Name, TotalSizeGb = _.TotalSize / 1024.0 / 1024.0 / 1024.0, AvailableFreeSpaceGb = _.AvailableFreeSpace / 1024.0 / 1024.0 / 1024.0, _.VolumeLabel, _.DriveFormat, DriveType = _.DriveType.ToString() }),
                        systemDirectory = Environment.SystemDirectory,
                        user = $"{Environment.UserDomainName}\\{Environment.UserName}",
                        process = System.Diagnostics.Process.GetCurrentProcess().MainModule
                    },
                    _ctx.HttpContext?.Request?.Headers,
                    remoteIpAddress = _ctx.HttpContext?.Connection?.RemoteIpAddress?.ToString()
                },
                cache = _cache.Keys != null ? _cache.Keys.Count() : 0,
                environment = new { _env.ApplicationName, _env.EnvironmentName, _env.ContentRootPath, _env.WebRootPath },
                config = new
                {
                    //builder = _config,
                    detail = _config?.AsEnumerable()
                        .Select(conf => new
                        {
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
                extensions =
                        ExtCore.Infrastructure.ExtensionManager.GetInstances<ExtCore.Infrastructure.Actions.IConfigureBuilder>()
                        .UnionInjector()
                        .Where(_ => _ is ExtCore.Infrastructure.ExtensionBase)
                        .Select(_ => new { (_ as ExtCore.Infrastructure.ExtensionBase).Name, _.Priority })
                    .OrderBy(_ => _.Priority),                
                services = AppInfo<TConfig>.Services?.Select(_ => new { 
                    Type = _.ServiceType, 
                    _.ImplementationType, 
                    Lifetime = _.Lifetime.ToString() })
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
            return Ok(new { Startup<TConfig>.Uptime, status = "Stopping" });
        }

    }
}
