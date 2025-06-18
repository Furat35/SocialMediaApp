using BuildingBlocks.Models;
using Microsoft.AspNetCore.Mvc;

namespace BuildingBlocks.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseController : ControllerBase
    {
        [NonAction]
        public IActionResult CreateActionResult<T>(ResponseDto<T> response)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return new ObjectResult(null) { StatusCode = (int)response.StatusCode };

            return new ObjectResult(response) { StatusCode = (int)response.StatusCode };
        }
    }
}
