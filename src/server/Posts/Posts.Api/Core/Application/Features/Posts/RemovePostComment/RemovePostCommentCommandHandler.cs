using AutoMapper;
using BuildingBlocks.Extensions;
using BuildingBlocks.Models;
using MediatR;
using Posts.Api.Core.Application.Repositories;
using System.Net;

namespace Posts.Api.Core.Application.Features.Posts.RemovePostComment
{
    public class RemovePostCommentCommandHandler(IPostRepository postRepository, IMapper mapper,
        IFollowerRepository followerRepository, IHttpContextAccessor httpContext)
        : IRequestHandler<RemovePostCommentCommand, ResponseDto<bool>>
    {
        public async Task<ResponseDto<bool>> Handle(RemovePostCommentCommand request, CancellationToken cancellationToken)
        {
            var post = await postRepository.GetByIdAsync(request.PostId, [_ => _.Comments]);
            if (post == null) return ResponseDto<bool>.Fail("Post not found", HttpStatusCode.NotFound);
            if (!await followerRepository.ActiveUserHasAccessToGivenUser(post.UserId))
            {
                return ResponseDto<bool>.Fail("You do not have permission to access this user's posts.", HttpStatusCode.Forbidden);
            }

            var commentToRemove = post.Comments.FirstOrDefault(_ => _.Id == request.CommentId);
            if (commentToRemove == null) return ResponseDto<bool>.Fail("Comment not found", HttpStatusCode.NotFound);
            if (commentToRemove.UserId != httpContext.GetUserId()) return ResponseDto<bool>.Fail("Forbidden!", HttpStatusCode.Forbidden);

            commentToRemove.IsValid = false;

            return ResponseDto<bool>.GenerateResponse(await postRepository.SaveChangesAsync() > 0)
               .Success(true, HttpStatusCode.OK)
               .Fail("An error occured while removing comment!", HttpStatusCode.InternalServerError);
        }
    }
}
