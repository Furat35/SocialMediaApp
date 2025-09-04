using BuildingBlocks.Controllers;
using BuildingBlocks.Models;
using IdentityServer.Api.Business.Dtos;
using IdentityServer.Api.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Api.Controllers
{
    public class AuthController(IAuthService authService) : BaseController
    {
        [HttpPost("register")]
        public async Task<ActionResult<ResponseDto<bool>>> Register([FromBody] RegisterRequestModel requestModel)
        {
            var response = await authService.RegisterAsync(requestModel);
            return CreateActionResult(response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<ResponseDto<LoginResponseModel>>> Login([FromBody] LoginRequestModel requestModel)
        {
            var response = await authService.LoginAsync(requestModel);
            return CreateActionResult(response);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<ResponseDto<LoginResponseModel>>> RefreshToken([FromBody] RefreshTokenRequestModel requestModel)
        {
            var response = await authService.RefreshTokenAsync(requestModel);
            return CreateActionResult(response);
        }
    }
}
