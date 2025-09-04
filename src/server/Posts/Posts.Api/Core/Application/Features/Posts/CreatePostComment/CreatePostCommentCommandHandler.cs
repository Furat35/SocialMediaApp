using AutoMapper;
using BuildingBlocks.Extensions;
using BuildingBlocks.Models;
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
        : BaseHandler<IPostRepository, Post>(postRepository),
            IRequestHandler<CreatePostCommentCommand, ResponseDto<bool>>
    {
        public async Task<ResponseDto<bool>> Handle(CreatePostCommentCommand request, CancellationToken cancellationToken)
        {
            var post = await _repository.GetFirstAsync(
                expression: _ => _.Id == request.PostId && _.UserId == request.FollowerId,
                includes: [_ => _.Comments]);

            if (post is null) return ToResponse<bool>(HttpStatusCode.NotFound);
            var comment = mapper.Map<Comment>(request);
            comment.UserId = httpContext.GetUserId();
            post.Comments.Add(comment);

            return await SaveChangesAsync();
        }
    }
}
