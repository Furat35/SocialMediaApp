using AutoMapper;
using BuildingBlocks.Extensions;
using BuildingBlocks.Models;
using BuildingBlocks.Models.Constants;
using MediatR;
using Posts.Api.Core.Application.Repositories;
using Posts.Api.Core.Domain.Entities;
using System.Net;

namespace Posts.Api.Core.Application.Features.Posts.CreatePostComment
{
    public class CreatePostCommentCommandHandler(
        IPostRepository postRepository,
        IHttpContextAccessor httpContext,
        IMapper mapper)
        : IRequestHandler<CreatePostCommentCommand, ResponseDto<bool>>
    {
        public async Task<ResponseDto<bool>> Handle(CreatePostCommentCommand request, CancellationToken cancellationToken)
        {
            var post = await postRepository.GetByIdAsync(request.PostId, [_ => _.Comments]);
            if (post is null) return ResponseDto<bool>.Fail(ErrorMessages.NotFound, HttpStatusCode.NotFound);
            if (post.UserId != request.FollowerId) return ResponseDto<bool>.Fail(ErrorMessages.Forbidden, HttpStatusCode.Forbidden);

            var comment = mapper.Map<Comment>(request);
            comment.UserId = httpContext.GetUserId();
            post.Comments.Add(comment);

            return ResponseDto<bool>.GenerateResponse(await postRepository.SaveChangesAsync() > 0)
               .Success(true, HttpStatusCode.OK)
               .Fail(ErrorMessages.CreateError, HttpStatusCode.InternalServerError);
        }
    }
}
