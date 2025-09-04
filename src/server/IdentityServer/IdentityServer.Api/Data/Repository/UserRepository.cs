using BuildingBlocks.Data;
using IdentityServer.Api.Data.Context;
using IdentityServer.Api.Models;

namespace IdentityServer.Api.Data.Repository
{
    public class UserRepository(IdentityDbContext context)
        : GenericRepository<AppUser, IdentityDbContext>(context), IUserRepository
    {
        public async Task<AppUser> GetByUsername(string username)
        {
            return await GetFirstAsync(u => u.Username == username);
        }
    }

    public interface IUserRepository : IGenericRepository<AppUser>
    {
        Task<AppUser> GetByUsername(string username);
    }
}
