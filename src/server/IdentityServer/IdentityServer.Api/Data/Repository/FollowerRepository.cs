using BuildingBlocks.Data;
using BuildingBlocks.Extensions;
using IdentityServer.Api.Core.Domain.Entities;
using IdentityServer.Api.Core.Domain.Enums;
using IdentityServer.Api.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Api.Infrastructure.Repositories
{
    public class FollowerRepository(IdentityDbContext context, IHttpContextAccessor httpContext) : GenericRepository<Follower, IdentityDbContext>(context)
    {
        public async Task<bool> IsFollowing(int userId1, int userId2)
        {
            return await Get(_ => _.Status == FollowStatus.Following && _.IsValid &&
                (_.RequestingUserId == userId1 && _.RespondingUserId == userId2 ||
                 _.RequestingUserId == userId2 && _.RespondingUserId == userId1))
                .AnyAsync();
        }

        public async Task<bool> ActiveUserHasAccessToGivenUser(int userId)
        {
            var isFollowing = await IsFollowing(userId, httpContext.GetUserId());
            return httpContext.GetUserId() == userId || isFollowing;
        }
    }
}
