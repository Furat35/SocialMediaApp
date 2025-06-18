using BuildingBlocks.Data;
using IdentityServer.Api.Data.Context;
using IdentityServer.Api.Models;

namespace IdentityServer.Api.Data.Repository
{
    public class UserRepository(IdentityDbContext context) : GenericRepository<AppUser, IdentityDbContext>(context)
    {
    }
}
