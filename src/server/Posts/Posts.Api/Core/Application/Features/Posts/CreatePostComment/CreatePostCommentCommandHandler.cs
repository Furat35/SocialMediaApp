using AutoMapper;
using BuildingBlocks.Extensions;
using BuildingBlocks.Models;
using MediatR;
using Posts.Api.Core.Application.Repositories;
using Posts.Api.Core.Domain.Entities;
using System.Net;

namespace Posts.Api.Core.Application.Features.Posts.CreatePostComment
{
    public class CreatePostCommentCommandHandler(IPostRepository postRepository, IMapper mapper,
         IFollowerRepository followerRepository, IHttpContextAccessor httpContext)
        : IRequestHandler<CreatePostCommentCommand, ResponseDto<bool>>
    {
        public async Task<ResponseDto<bool>> Handle(CreatePostCommentCommand request, CancellationToken cancellationToken)
        {
            var post = await postRepository.GetByIdAsync(request.PostId, [_ => _.Comments]);
            if (post == null) return ResponseDto<bool>.Fail("Post not found", HttpStatusCode.NotFound);
            if (!await followerRepository.ActiveUserHasAccessToGivenUser(post.UserId))
                return ResponseDto<bool>.Fail("You do not have permission to access this user's posts.", HttpStatusCode.Forbidden);

            var comment = mapper.Map<Comment>(request);
            comment.UserId = httpContext.GetUserId();
            post.Comments.Add(comment);

            return ResponseDto<bool>.GenerateResponse(await postRepository.SaveChangesAsync() > 0)
               .Success(true, HttpStatusCode.OK)
               .Fail("An error occured while adding comment!", HttpStatusCode.InternalServerError);
        }
    }
}
