using BuildingBlocks.Extensions;
using BuildingBlocks.Models;
using MediatR;
using Posts.Api.Core.Application.Repositories;
using Posts.Api.Core.Domain.Entities;
using System.Net;

namespace Posts.Api.Core.Application.Features.Posts.UnlikePost
{
    public class UnlikePostCommandHandler(
        IPostRepository postRepository,
        IHttpContextAccessor httpContext)
        : BaseHandler<IPostRepository, Post>(postRepository),
            IRequestHandler<UnlikePostCommand, ResponseDto<bool>>
    {
        public async Task<ResponseDto<bool>> Handle(UnlikePostCommand request, CancellationToken cancellationToken)
        {
            var post = await _repository.GetFirstAsync(_ =>
                _.Id == request.PostId && _.UserId == request.FollowerId && _.IsValid,
                [_ => _.Likes]);
            if (post is null) return ToResponse<bool>(HttpStatusCode.NotFound);

            var likeToRemove = _repository.LikeExists(post, httpContext.GetUserId());
            if (likeToRemove is null) return ToResponse<bool>(HttpStatusCode.NotFound);

            post.Likes.Remove(likeToRemove);

            return await SaveChangesAsync();
        }
    }
}
