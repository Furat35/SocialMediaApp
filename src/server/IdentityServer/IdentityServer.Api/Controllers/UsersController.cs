using BuildingBlocks.Controllers;
using IdentityServer.Api.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Api.Controllers
{
    public class UsersController(IUserService userService) : BaseController
    {
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            var user = await userService.GetUserById(userId);
            return CreateActionResult(user);
        }

        [HttpPost]
        public async Task<IActionResult> GetUsersById([FromBody] List<int> userIds)
        {
            var user = await userService.GetUsersById(userIds);
            return CreateActionResult(user);
        }
    }
}
