using Microsoft.AspNetCore.Mvc;
using Ws.Core.Extensions.Data.Cache;

namespace x.core.Controllers
{

    [ApiController]
    //[ApiExplorerSettings(GroupName = "admin")]
    [Route("api/controller/[controller]")]
    //[Authorize(Policy = nameof(UserRole.Admin))]
    public class Exception : ControllerBase
    {
        [HttpGet("argumentOutOfRange/{name}/{value}")]
        [ProducesResponseType(typeof(System.Exception), 200)]
        public IActionResult GetArgumentOutOfRangeException(string name, string value)
        {
            System.Exception ex = new x.core.Endpoints.Exception().GetArgumentOutOfRangeException(name, value);
            return Ok(ex);
        }
    }
}