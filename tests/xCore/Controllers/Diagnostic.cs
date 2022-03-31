using Microsoft.AspNetCore.Mvc;
using Ws.Core.Extensions.Data.Cache;

namespace xCore.Controllers
{

    [ApiController]
    [ApiExplorerSettings(GroupName = "admin")]
    [Route("api/[controller]")]
    //[Authorize(Policy = nameof(UserRole.Admin))]
    public class Diagnostic : Ws.Core.Extensions.Api.Controllers.DiagnosticController<Ws.Core.AppConfig>
    {
        public Diagnostic(
            ICache cache,
            IConfiguration config,
            IWebHostEnvironment env,
            IHostApplicationLifetime applicationLifetime,
            IHttpContextAccessor ctx) :
            base(
                cache,
                config,
                env,
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
