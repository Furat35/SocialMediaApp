using AutoMapper;
using BuildingBlocks.Extensions;
using BuildingBlocks.Models;
using MediatR;
using Posts.Api.Core.Application.Repositories;
using Posts.Api.Core.Domain.Entities;
using System.Net;

namespace Posts.Api.Core.Application.Features.Posts.UpdatePost
{
    public class UpdatePostCommandHandler(
        IPostRepository postRepository,
        IMapper mapper,
        IHttpContextAccessor httpContext)
        : BaseHandler<IPostRepository, Post>(postRepository),
            IRequestHandler<UpdatePostCommand, ResponseDto<bool>>
    {
        public async Task<ResponseDto<bool>> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
        {
            var post = await _repository.GetPostAsync(request.Id, httpContext.GetUserId());
            if (post is null) return ToResponse<bool>(HttpStatusCode.NotFound);

            mapper.Map(request, post);

            return await SaveChangesAsync();
        }
    }
}
