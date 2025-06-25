using BuildingBlocks.Models;
using IdentityServer.Api.Business.Interfaces;
using IdentityServer.Api.Data.Context;
using IdentityServer.Api.Data.Repository;
using IdentityServer.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace IdentityServer.Api.Business
{
    public class UserService(IdentityDbContext context) : UserRepository(context), IUserService
    {
        public async Task<ResponseDto<AppUser>> GetUserById(int userId)
        {
            var user = await GetByIdAsync(userId);
            return ResponseDto<AppUser>.Success(user, HttpStatusCode.OK);
        }

        public async Task<ResponseDto<List<AppUser>>> GetUsersById(List<int> userIds)
        {
            var users = await Get(_ => userIds.Contains( _.Id)).ToListAsync();
            return ResponseDto<List<AppUser>>.Success(users, HttpStatusCode.OK);
        }
    }
}
