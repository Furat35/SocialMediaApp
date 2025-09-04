using BuildingBlocks.Data;
using Posts.Api.Core.Domain.Entities;

namespace Posts.Api.Core.Application.Repositories
{
    public interface IPostRepository
        : IGenericRepository<Post>
    {
        Like LikeExists(Post post, int userId);
        Task<Post> GetPostAsync(int postId, int userId);
    }
}
