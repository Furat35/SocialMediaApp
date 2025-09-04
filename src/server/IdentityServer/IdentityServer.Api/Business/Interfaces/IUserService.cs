using BuildingBlocks.Models;
using IdentityServer.Api.Business.Dtos;
using IdentityServer.Api.Business.Dtos.AppUsers;
using IdentityServer.Api.Models;

namespace IdentityServer.Api.Business.Interfaces
{
    public interface IUserService
    {
        Task<PaginationResponseModel<AppUser>> GetUsers(UserRequestDto request);
        Task<ResponseDto<AppUser>> GetUserById(int userId);
        Task<ResponseDto<List<AppUser>>> GetUsersById(List<int> userIds);
        Task<ResponseDto<List<int>>> SearchedUserIds(UserRequestDto request);
        Task<ResponseDto<bool>> UpdateUser(AppUserUpdateDto updateDto);
        Task<(byte[] image, string fileType)> GetUserImage(int userId);
    }
}
