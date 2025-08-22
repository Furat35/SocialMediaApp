using BuildingBlocks.Extensions;
using BuildingBlocks.Models;
using BuildingBlocks.Models.Constants;
using MediatR;
using Posts.Api.Core.Application.Repositories;
using System.Net;

namespace Posts.Api.Core.Application.Features.Posts.DeletePost
{
    public class DeletePostCommandHandler(
        IPostRepository postRepository,
        IHttpContextAccessor httpContext)
        : IRequestHandler<DeletePostCommand, ResponseDto<bool>>
    {
        public async Task<ResponseDto<bool>> Handle(DeletePostCommand request, CancellationToken cancellationToken)
        {
            var post = await postRepository.GetByIdAsync(request.Id);
            if (post is null) return ResponseDto<bool>.Fail(ErrorMessages.NotFound, HttpStatusCode.NotFound);
            if (post.UserId != httpContext.GetUserId()) return ResponseDto<bool>.Fail(ErrorMessages.NotFound, HttpStatusCode.Forbidden);

            post.IsValid = false;

            return ResponseDto<bool>.GenerateResponse(await postRepository.SaveChangesAsync() > 0)
                .Success(true, HttpStatusCode.OK)
                .Fail(ErrorMessages.DeleteError, HttpStatusCode.InternalServerError);
        }
    }
}
