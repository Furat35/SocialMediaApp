using IdentityServer.Api.Business.Interfaces;
using IdentityServer.Api.Data.Context;
using IdentityServer.Api.Data.Repository;

namespace IdentityServer.Api.Business
{
    public class UserService(IdentityDbContext context) : UserRepository(context), IUserService
    {
    }
}
