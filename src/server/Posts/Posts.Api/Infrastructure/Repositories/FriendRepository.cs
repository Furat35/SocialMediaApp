using BuildingBlocks.Data;
using Posts.Api.Core.Application.Repositories;
using Posts.Api.Core.Domain.Entities;

namespace Posts.Api.Infrastructure.Repositories
{
    public class FriendRepository(PostDbContext context) : GenericRepository<Friend, PostDbContext>(context), IFriendRepository
    {
    }
}
