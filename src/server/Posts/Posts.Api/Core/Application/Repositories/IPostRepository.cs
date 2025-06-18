using BuildingBlocks.Data;
using Posts.Api.Core.Domain.Entities;

namespace Posts.Api.Core.Application.Repositories
{
    public interface IPostRepository : IGenericRepository<Post>
    {
    }
}
