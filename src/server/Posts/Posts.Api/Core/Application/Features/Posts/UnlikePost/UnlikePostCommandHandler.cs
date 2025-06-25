using AutoMapper;
using BuildingBlocks.Extensions;
using BuildingBlocks.Models;
using MediatR;
using Posts.Api.Core.Application.Repositories;
using System.Net;

namespace Posts.Api.Core.Application.Features.Posts.UnlikePost
{
    public class UnlikePostCommandHandler(IPostRepository postRepository, IMapper mapper,
        IHttpContextAccessor httpContext)
        : IRequestHandler<UnlikePostCommand, ResponseDto<bool>>
    {
        public async Task<ResponseDto<bool>> Handle(UnlikePostCommand request, CancellationToken cancellationToken)
        {
            var post = await postRepository.GetByIdAsync(request.PostId, [_ => _.Likes]);
            if (post == null) return ResponseDto<bool>.Fail("Post not found", HttpStatusCode.NotFound);

            var likeToRemove = post.Likes.First(_ => _.UserId == httpContext.GetUserId());
            if (likeToRemove == null) return ResponseDto<bool>.Fail("Not found", HttpStatusCode.NotFound);

            post.Likes.Remove(likeToRemove);

            return ResponseDto<bool>.GenerateResponse(await postRepository.SaveChangesAsync() > 0)
               .Success(true, HttpStatusCode.OK)
               .Fail("An error occured while removing comment!", HttpStatusCode.InternalServerError);
        }
    }
}
