using BuildingBlocks.Data;
using BuildingBlocks.Models;
using IdentityServer.Api.Business.Dtos.AppUsers;
using IdentityServer.Api.Models;

namespace IdentityServer.Api.Business.Interfaces
{
    public interface IUserService : IGenericRepository<AppUser>
    {
        Task<ResponseDto<AppUser>> GetUserById(int userId);
        Task<ResponseDto<List<AppUser>>> GetUsersById(List<int> userIds);
        Task<ResponseDto<bool>> UpdateUser(AppUserUpdateDto updateDto);
        Task<(byte[] image, string fileType)> GetUserImage(int userId);
    }
}
