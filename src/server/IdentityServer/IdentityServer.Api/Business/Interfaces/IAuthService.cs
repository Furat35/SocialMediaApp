using BuildingBlocks.Models;
using IdentityServer.Api.Business.Dtos;

namespace IdentityServer.Api.Business.Interfaces
{
    public interface IAuthService
    {
        Task<ResponseDto<LoginResponseModel>> LoginAsync(LoginRequestModel loginRequest);
        Task<ResponseDto<bool>> RegisterAsync(RegisterRequestModel registerModel);
        Task<ResponseDto<LoginResponseModel>> RefreshTokenAsync(RefreshTokenRequestModel request);
        string HashPassword(string password, byte[] salt, int iterations = 10000, int hashByteSize = 32);
    }
}
