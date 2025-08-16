using AutoMapper;
using BuildingBlocks.Extensions;
using BuildingBlocks.Models;
using MediatR;
using Posts.Api.Core.Application.Repositories;
using Posts.Api.Core.Domain.Entities;
using System.Net;

namespace Posts.Api.Core.Application.Features.Posts.LikePost
{
    public class LikePostCommandHandler(IPostRepository postRepository, IMapper mapper,
        IFollowerRepository followerRepository, IHttpContextAccessor httpContext)
        : IRequestHandler<LikePostCommand, ResponseDto<bool>>
    {
        public async Task<ResponseDto<bool>> Handle(LikePostCommand request, CancellationToken cancellationToken)
        {
            var post = await postRepository.GetByIdAsync(request.PostId, [_ => _.Likes]);
            if (post == null) return ResponseDto<bool>.Fail("Post not found", HttpStatusCode.NotFound);
            if (!await followerRepository.ActiveUserHasAccessToGivenUser(post.UserId))
            {
                return ResponseDto<bool>.Fail("You do not have permission to access this user's posts.", HttpStatusCode.Forbidden);
            }
            if (post.Likes.Any(_ => _.UserId == httpContext.GetUserId()))
                return ResponseDto<bool>.Success(true, HttpStatusCode.OK);

            post.Likes.Add(new Like
            {
                UserId = httpContext.GetUserId()
            });

            return ResponseDto<bool>.GenerateResponse(await postRepository.SaveChangesAsync() > 0)
               .Success(true, HttpStatusCode.OK)
               .Fail("An error occured while removing comment!", HttpStatusCode.InternalServerError);
        }
    }
}
