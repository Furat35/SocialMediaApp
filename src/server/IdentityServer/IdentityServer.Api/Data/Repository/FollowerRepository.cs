using BuildingBlocks.Data;
using BuildingBlocks.Extensions;
using IdentityServer.Api.Core.Domain.Entities;
using IdentityServer.Api.Core.Domain.Enums;
using IdentityServer.Api.Data.Context;
using Microsoft.EntityFrameworkCore;
namespace IdentityServer.Api.Infrastructure.Repositories
{
    public class FollowerRepository(IdentityDbContext context, IHttpContextAccessor httpContext)
        : GenericRepository<Follower, IdentityDbContext>(context), IFollowerRepository
    {
        public async Task<Follower?> GetFollowerByIdAsync(int userId)
        {
            return await Get(f =>
                (f.RequestingUserId == userId && f.RespondingUserId == httpContext.GetUserId() ||
                    f.RespondingUserId == userId && f.RequestingUserId == httpContext.GetUserId()) &&
                    f.Status == FollowStatus.Following && f.IsValid)
             .FirstOrDefaultAsync();
        }

        public async Task<int> GetFollowersCountAsync(int userId)
        {
            return await Get(_ => (_.RequestingUserId == userId ||
                       _.RespondingUserId == userId) &&
                       _.IsValid && _.Status == FollowStatus.Following)
                   .CountAsync();
        }
        public async Task<Follower> GetFollowerWithGivenStatusAsync(int requestingUserId, int respondingUserId, FollowStatus status)
        {
            return await Get(_ => _.RequestingUserId == requestingUserId && _.RespondingUserId == respondingUserId &&
                   _.Status == status && _.IsValid)
                .FirstOrDefaultAsync();
        }

        public async Task<Follower?> FollowExistsAsync(int userId)
        {
            return await Get(f =>
                           ((f.RequestingUserId == httpContext.GetUserId() && f.RespondingUserId == userId) ||
                            (f.RequestingUserId == userId && f.RespondingUserId == httpContext.GetUserId())) &&
                            f.IsValid)
                .OrderByDescending(_ => _.CreateDate)
                .FirstOrDefaultAsync();
        }
    }

    public interface IFollowerRepository : IGenericRepository<Follower>
    {
        Task<Follower?> GetFollowerByIdAsync(int id);
        Task<int> GetFollowersCountAsync(int userId);
        Task<Follower> GetFollowerWithGivenStatusAsync(int requestingUserId, int respondingUserId, FollowStatus status);
        Task<Follower?> FollowExistsAsync(int userId);
    }
}
