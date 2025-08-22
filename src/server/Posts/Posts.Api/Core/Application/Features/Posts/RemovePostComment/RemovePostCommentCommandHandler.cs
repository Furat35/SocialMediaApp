using BuildingBlocks.Extensions;
using BuildingBlocks.Models;
using BuildingBlocks.Models.Constants;
using MediatR;
using Posts.Api.Core.Application.Repositories;
using System.Net;

namespace Posts.Api.Core.Application.Features.Posts.RemovePostComment
{
    public class RemovePostCommentCommandHandler(
        IPostRepository postRepository,
        IHttpContextAccessor httpContext)
        : IRequestHandler<RemovePostCommentCommand, ResponseDto<bool>>
    {
        public async Task<ResponseDto<bool>> Handle(RemovePostCommentCommand request, CancellationToken cancellationToken)
        {
            var post = await postRepository.GetByIdAsync(request.PostId, [_ => _.Comments]);
            if (post is null) return ResponseDto<bool>.Fail(ErrorMessages.NotFound, HttpStatusCode.NotFound);
            if (post.UserId != request.FollowerId) return ResponseDto<bool>.Fail(ErrorMessages.Forbidden, HttpStatusCode.Forbidden);

            var commentToRemove = post.Comments.FirstOrDefault(_ => _.Id == request.CommentId);
            if (commentToRemove is null) return ResponseDto<bool>.Fail(ErrorMessages.NotFound, HttpStatusCode.NotFound);
            if (commentToRemove.UserId != httpContext.GetUserId()) return ResponseDto<bool>.Fail(ErrorMessages.Forbidden, HttpStatusCode.Forbidden);

            commentToRemove.IsValid = false;

            return ResponseDto<bool>.GenerateResponse(await postRepository.SaveChangesAsync() > 0)
               .Success(true, HttpStatusCode.OK)
               .Fail(ErrorMessages.DeleteError, HttpStatusCode.InternalServerError);
        }
    }
}
