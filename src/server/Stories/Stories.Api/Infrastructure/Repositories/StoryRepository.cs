using BuildingBlocks.Data;
using Stories.Api.Core.Application.Repositories;
using Stories.Api.Core.Domain.Entities;

namespace Stories.Api.Infrastructure.Repositories
{
    public class StoryRepository(StoryDbContext dbContext)
        : GenericRepository<Story, StoryDbContext>(dbContext), IStoryRepository
    {

    }
}
