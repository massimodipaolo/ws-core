using Microsoft.AspNetCore.Mvc;
using Ws.Core.Extensions.Data.Cache;

namespace x.core.Controllers
{
    
    [ApiController]
    //[ApiExplorerSettings(GroupName = "admin")]
    [Route("api/[controller]")]
    //[Authorize(Policy = nameof(UserRole.Admin))]
    public class Diagnostic : ControllerBase
    {
        //[Authorize(Policy = nameof(UserRole.Admin))]
        [HttpGet]
        [ProducesResponseType(typeof(Ws.Core.Extensions.Diagnostic.AppRuntime), 200)]
        public IActionResult Get(
            [FromServices] ICache cache,
            [FromServices] IConfiguration config,
            [FromServices] IWebHostEnvironment env,
            [FromServices] IHttpContextAccessor ctx
            ) => Ok(Ws.Core.Extensions.Diagnostic.AppRuntime<Ws.Core.AppConfig>.Get(cache, config, env, ctx));

    }

}
