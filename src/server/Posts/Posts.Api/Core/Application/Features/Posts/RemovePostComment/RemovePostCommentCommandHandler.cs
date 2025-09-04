using BuildingBlocks.Extensions;
using BuildingBlocks.Models;
using MediatR;
using Posts.Api.Core.Application.Repositories;
using Posts.Api.Core.Domain.Entities;
using System.Net;

namespace Posts.Api.Core.Application.Features.Posts.RemovePostComment
{
    public class RemovePostCommentCommandHandler(
        IPostRepository postRepository,
        IHttpContextAccessor httpContext)
        : BaseHandler<IPostRepository, Post>(postRepository),
            IRequestHandler<RemovePostCommentCommand, ResponseDto<bool>>
    {
        public async Task<ResponseDto<bool>> Handle(RemovePostCommentCommand request, CancellationToken cancellationToken)
        {
            var post = await _repository.GetFirstAsync(
                _ => _.Id == request.PostId && _.UserId == request.FollowerId && _.IsValid,
                [_ => _.Comments]);
            if (post is null) return ToResponse<bool>(HttpStatusCode.NotFound);

            var commentToRemove = post.Comments.FirstOrDefault(_ =>
                _.Id == request.CommentId && _.UserId != httpContext.GetUserId() && _.IsValid);
            if (commentToRemove is null) return ToResponse<bool>(HttpStatusCode.NotFound);

            commentToRemove.IsValid = false;

            return await SaveChangesAsync();
        }
    }
}
