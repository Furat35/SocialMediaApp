using BuildingBlocks.Data;
using IdentityServer.Api.Models;

namespace IdentityServer.Api.Business.Interfaces
{
    public interface IUserService : IGenericRepository<AppUser>
    {
    }
}
