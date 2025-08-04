using BuildingBlocks.Data;
using BuildingBlocks.Extensions;
using Microsoft.EntityFrameworkCore;
using Posts.Api.Core.Application.Repositories;
using Posts.Api.Core.Domain.Entities;
using Posts.Api.Core.Domain.Enums;

namespace Posts.Api.Infrastructure.Repositories
{
    public class FollowerRepository(PostDbContext context, IHttpContextAccessor httpContext) : GenericRepository<Follower, PostDbContext>(context), IFollowerRepository
    {
        public async Task<bool> IsFollowing(int userId1, int userId2)
        {
            return await Get(_ => _.Status == FollowStatus.Following && _.IsValid &&
                (_.RequestingUserId == userId1 && _.RespondingUserId == userId2 ||
                 _.RequestingUserId == userId2 && _.RespondingUserId == userId1))
                .AnyAsync();
        }

        public async Task<bool> ActiveUserHasAccessToGivenUsersPosts(int userId)
        {
            var isFollowing = await IsFollowing(userId, httpContext.GetUserId());
            return httpContext.GetUserId() == userId || isFollowing;
        }
    }
}
