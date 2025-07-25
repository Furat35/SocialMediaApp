using BuildingBlocks.Controllers;
using IdentityServer.Api.Business.Dtos;
using IdentityServer.Api.Business.Dtos.AppUsers;
using IdentityServer.Api.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Api.Controllers
{
    public class UsersController(IUserService userService) : BaseController
    {
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] UserRequestDto request)
        {
            var users = await userService.GetUsers(request);
            return Ok(users);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById([FromRoute] int userId)
        {
            var user = await userService.GetUserById(userId);
            return CreateActionResult(user);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchUsers([FromQuery] UserRequestDto request)
        {
            var users = await userService.SearchedUserIds(request);
            return Ok(users);
        }


        [HttpPost]
        public async Task<IActionResult> GetUsersById([FromBody] List<int> userIds)
        {
            var user = await userService.GetUsersById(userIds);
            return CreateActionResult(user);
        }

        [HttpPost("update")]
        public async Task<IActionResult> GetUsersById([FromForm] AppUserUpdateDto updateDto)
        {
            var user = await userService.UpdateUser(updateDto);
            return CreateActionResult(user);
        }

        [HttpGet("image")]
        public async Task<IActionResult> GetUserImage([FromQuery] int userId)
        {
            var response = await userService.GetUserImage(userId);
            return File(response.image, response.fileType);
        }
    }
}
