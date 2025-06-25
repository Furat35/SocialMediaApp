using BuildingBlocks.Data;
using BuildingBlocks.Models;
using IdentityServer.Api.Models;

namespace IdentityServer.Api.Business.Interfaces
{
    public interface IUserService : IGenericRepository<AppUser>
    {
        Task<ResponseDto<AppUser>> GetUserById(int userId);
        Task<ResponseDto<List<AppUser>>> GetUsersById(List<int> userIds);
    }
}
