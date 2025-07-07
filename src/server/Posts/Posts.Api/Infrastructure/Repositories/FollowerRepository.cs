using BuildingBlocks.Data;
using Posts.Api.Core.Application.Repositories;
using Posts.Api.Core.Domain.Entities;

namespace Posts.Api.Infrastructure.Repositories
{
    public class FollowerRepository(PostDbContext context) : GenericRepository<Follower, PostDbContext>(context), IFollowerRepository
    {
    }
}
