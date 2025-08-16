using BuildingBlocks.Data;
using Posts.Api.Core.Application.Repositories;
using Posts.Api.Core.Domain.Entities;

namespace Posts.Api.Infrastructure.Repositories
{
    public class StoryRepository(PostDbContext dbContext)
        : GenericRepository<Story, PostDbContext>(dbContext), IStoryRepository
    {

    }
}
