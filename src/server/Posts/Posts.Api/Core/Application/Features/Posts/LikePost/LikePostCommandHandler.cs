using BuildingBlocks.Extensions;
using BuildingBlocks.Models;
using MediatR;
using Posts.Api.Core.Application.Repositories;
using Posts.Api.Core.Domain.Entities;
using System.Net;

namespace Posts.Api.Core.Application.Features.Posts.LikePost
{
    public class LikePostCommandHandler(
        IPostRepository postRepository,
        IHttpContextAccessor httpContext)
        : BaseHandler<IPostRepository, Post>(postRepository),
            IRequestHandler<LikePostCommand, ResponseDto<bool>>
    {
        public async Task<ResponseDto<bool>> Handle(LikePostCommand request, CancellationToken cancellationToken)
        {
            var post = await _repository.GetFirstAsync(
                _ => _.Id == request.PostId && _.UserId == request.FollowerId && _.IsValid,
                [_ => _.Likes]);
            if (post is null) return ToResponse<bool>(HttpStatusCode.NotFound);

            if (_repository.LikeExists(post, httpContext.GetUserId()) is not null)
                return ReturnSuccess(true);

            post.Likes.Add(new Like { UserId = httpContext.GetUserId() });

            return await SaveChangesAsync();
        }
    }
}
