using BuildingBlocks.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Posts.Api.Core.Application.Repositories;
using Posts.Api.Core.Domain.Entities;
using BuildingBlocks.Extensions;

namespace Posts.Api.Infrastructure.Repositories
{
    public class PostRepository(PostDbContext context, IHttpContextAccessor httpContext)
        : GenericRepository<Post, PostDbContext>(context), IPostRepository
    {
        public Like LikeExists(Post post, int userId)
        {
            return post.Likes.FirstOrDefault(_ => _.UserId == httpContext.GetUserId());
        }

        public async Task<Post> GetPostAsync(int postId, int userId)
        {
            return await GetFirstAsync(_ => _.Id == postId && _.UserId == userId);
        }
    }
}



