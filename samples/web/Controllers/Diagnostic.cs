using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ws.Core.Extensions;
using Ws.Core.Extensions.Data.Cache;

namespace web.Controllers
{
    [ApiController]
    [ApiExplorerSettings(GroupName = "admin")]
    [Route("api/[controller]")]
    //[Authorize(Policy = nameof(UserRole.Admin))]
    public class Diagnostic: Ws.Core.Extensions.Api.Controllers.DiagnosticController<Ws.Core.AppConfig>
    {
        public Diagnostic(
            ICache cache,
            IConfiguration config,
            IWebHostEnvironment env,
            IHostApplicationLifetime applicationLifetime,
            IHttpContextAccessor ctx
            ) : base(cache, config, env, applicationLifetime, ctx)
        {
        }

        /// <summary>
        /// Return system and application diagnostic informations, about the environment, app configuration, extentions injected, service implementations
        /// </summary>
        /// <returns></returns> 
        //[Authorize(Policy = nameof(UserRole.Admin))]
        [HttpGet]
        public override IActionResult Get() => base.Get();

        //[Authorize(Policy = nameof(UserRole.Supervisor))]
        [HttpGet]
        [Route("Stop")]
        public override async Task<IActionResult> Stop() => await base.Stop();
    }
}
