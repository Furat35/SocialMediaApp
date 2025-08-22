using BuildingBlocks.Extensions;
using BuildingBlocks.Models;
using BuildingBlocks.Models.Constants;
using MediatR;
using Posts.Api.Core.Application.Repositories;
using System.Net;

namespace Posts.Api.Core.Application.Features.Posts.UnlikePost
{
    public class UnlikePostCommandHandler(
        IPostRepository postRepository,
        IHttpContextAccessor httpContext)
        : IRequestHandler<UnlikePostCommand, ResponseDto<bool>>
    {
        public async Task<ResponseDto<bool>> Handle(UnlikePostCommand request, CancellationToken cancellationToken)
        {
            var post = await postRepository.GetByIdAsync(request.PostId, [_ => _.Likes]);
            if (post == null) return ResponseDto<bool>.Fail(ErrorMessages.NotFound, HttpStatusCode.NotFound);
            if (post.UserId != request.FollowerId) return ResponseDto<bool>.Fail(ErrorMessages.Forbidden, HttpStatusCode.Forbidden);

            var likeToRemove = post.Likes.First(_ => _.UserId == httpContext.GetUserId());
            if (likeToRemove == null) return ResponseDto<bool>.Fail(ErrorMessages.NotFound, HttpStatusCode.NotFound);

            post.Likes.Remove(likeToRemove);

            return ResponseDto<bool>.GenerateResponse(await postRepository.SaveChangesAsync() > 0)
               .Success(true, HttpStatusCode.OK)
               .Fail(ErrorMessages.DeleteError, HttpStatusCode.InternalServerError);
        }
    }
}
