using BuildingBlocks.Data;
using Posts.Api.Core.Domain.Entities;

namespace Posts.Api.Core.Application.Repositories
{
    public interface IFollowerRepository : IGenericRepository<Follower>
    {
        Task<bool> IsFollowing(int userId1, int userId2);
        Task<bool> ActiveUserHasAccessToGivenUser(int userId);
    }
}
