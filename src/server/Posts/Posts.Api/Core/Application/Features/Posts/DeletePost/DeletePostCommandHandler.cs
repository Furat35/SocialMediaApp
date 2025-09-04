using BuildingBlocks.Extensions;
using BuildingBlocks.Models;
using MediatR;
using Posts.Api.Core.Application.Repositories;
using Posts.Api.Core.Domain.Entities;
using System.Net;

namespace Posts.Api.Core.Application.Features.Posts.DeletePost
{
    public class DeletePostCommandHandler(
        IPostRepository postRepository,
        IHttpContextAccessor httpContext)
        : BaseHandler<IPostRepository, Post>(postRepository),
            IRequestHandler<DeletePostCommand, ResponseDto<bool>>
    {
        public async Task<ResponseDto<bool>> Handle(DeletePostCommand request, CancellationToken cancellationToken)
        {
            var post = await _repository.GetFirstAsync(_ => _.Id == request.Id && _.UserId == httpContext.GetUserId());
            if (post is null) return ToResponse<bool>(HttpStatusCode.NotFound);

            post.IsValid = false;

            return await SaveChangesAsync();
        }
    }
}
