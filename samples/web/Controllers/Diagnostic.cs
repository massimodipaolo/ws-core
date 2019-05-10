using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ws.Core.Extensions.Data.Cache;

namespace web.Controllers
{
    [ApiExplorerSettings(GroupName = "admin")]
    [Route("api/[controller]")]
    //[Authorize(Policy = nameof(UserRole.Admin))]
    public class Diagnostic : Ws.Core.Extensions.Api.Controllers.DiagnosticController<AppConfig>
    {
        public Diagnostic(
            ICache cache,
            IConfiguration config,
            IHostingEnvironment env,
            IOptionsMonitor<AppConfig> appConfigMonitor,
            IOptionsMonitor<Ws.Core.Extensions.Base.Configuration> extConfigMonitor,
            IApplicationLifetime applicationLifetime,
            IHttpContextAccessor ctx) :
            base(
                cache,
                config,
                env,
                appConfigMonitor,
                extConfigMonitor,
                applicationLifetime,
                ctx)
        { }

        //[Authorize(Policy = nameof(UserRole.Admin))]
        [HttpGet]
        public override IActionResult Get() => base.Get();

        //[Authorize(Policy = nameof(UserRole.Supervisor))]
        [HttpGet]
        [Route("Stop")]
        public override async Task<IActionResult> Stop() => await base.Stop();

    }
}
