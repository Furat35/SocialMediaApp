using BuildingBlocks.Models;
using Microsoft.AspNetCore.Mvc;

namespace SocialMediaApp.Aggregator.Controllers
{
    [Route("api/aggregated/[controller]")]
    [ApiController]
    public class AggregatedBaseController : ControllerBase
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
