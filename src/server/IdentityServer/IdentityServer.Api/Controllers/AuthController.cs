using BuildingBlocks.Controllers;
using IdentityServer.Api.Business.Dtos;
using IdentityServer.Api.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Api.Controllers
{
    public class AuthController(IAuthService authService) : BaseController
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestModel requestModel)
        {
            var response = await authService.RegisterAsync(requestModel);
            return CreateActionResult(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestModel requestModel)
        {
            var response = await authService.LoginAsync(requestModel);
            return CreateActionResult(response);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestModel requestModel)
        {
            var response = await authService.RefreshTokenAsync(requestModel);
            return CreateActionResult(response);
        }
    }
}
