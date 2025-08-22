using BuildingBlocks.Extensions;
using BuildingBlocks.Models;
using BuildingBlocks.Models.Constants;
using MediatR;
using Posts.Api.Core.Application.Repositories;
using Posts.Api.Core.Domain.Entities;
using System.Net;

namespace Posts.Api.Core.Application.Features.Posts.LikePost
{
    public class LikePostCommandHandler(
        IPostRepository postRepository,
        IHttpContextAccessor httpContext)
        : IRequestHandler<LikePostCommand, ResponseDto<bool>>
    {
        public async Task<ResponseDto<bool>> Handle(LikePostCommand request, CancellationToken cancellationToken)
        {
            var post = await postRepository.GetByIdAsync(request.PostId, [_ => _.Likes]);
            if (post == null) return ResponseDto<bool>.Fail(ErrorMessages.NotFound, HttpStatusCode.NotFound);
            if (post.UserId != request.FollowerId) return ResponseDto<bool>.Fail(ErrorMessages.Forbidden, HttpStatusCode.Forbidden);
            if (post.Likes.Any(_ => _.UserId == httpContext.GetUserId()))
                return ResponseDto<bool>.Success(true, HttpStatusCode.OK);

            post.Likes.Add(new Like { UserId = httpContext.GetUserId() });

            return ResponseDto<bool>.GenerateResponse(await postRepository.SaveChangesAsync() > 0)
               .Success(true, HttpStatusCode.OK)
               .Fail(ErrorMessages.DeleteError, HttpStatusCode.InternalServerError);
        }
    }
}
