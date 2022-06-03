using Microsoft.AspNetCore.Mvc;
using Ws.Core.Extensions.Data.Cache;

namespace xCore.Controllers
{
    
    [ApiController]
    [ApiExplorerSettings(GroupName = "admin")]
    [Route("api/[controller]")]
    //[Authorize(Policy = nameof(UserRole.Admin))]
    public class Diagnostic : Ws.Core.Extensions.Api.Controllers.BaseController
    {

        public Diagnostic(IHttpContextAccessor ctx) :
            base(ctx)
        { }

        //[Authorize(Policy = nameof(UserRole.Admin))]
        [HttpGet]
        [ProducesResponseType(typeof(Ws.Core.Extensions.Diagnostic.AppRuntime), 200)]
        public IActionResult Get(
            [FromServices] ICache cache,
            [FromServices] IConfiguration config,
            [FromServices] IWebHostEnvironment env
            ) => Ok(Ws.Core.Extensions.Diagnostic.AppRuntime<Ws.Core.AppConfig>.Get(cache, config, env, _ctx));

    }

}
