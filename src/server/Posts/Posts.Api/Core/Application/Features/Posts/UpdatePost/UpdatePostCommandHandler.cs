using AutoMapper;
using BuildingBlocks.Extensions;
using BuildingBlocks.Models;
using MediatR;
using Posts.Api.Core.Application.Repositories;
using System.Net;

namespace Posts.Api.Core.Application.Features.Posts.UpdatePost
{
    public class UpdatePostCommandHandler(IPostRepository postRepository, IMapper mapper,
        IHttpContextAccessor httpContext)
        : IRequestHandler<UpdatePostCommand, ResponseDto<bool>>
    {
        public async Task<ResponseDto<bool>> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
        {
            var post = await postRepository.GetById(request.Id);
            if (post == null) return ResponseDto<bool>.Fail("Post not found", HttpStatusCode.NotFound);
            if (post.UserId != httpContext.GetUserId()) return ResponseDto<bool>.Fail("Forbidden", HttpStatusCode.Forbidden);

            mapper.Map(request, post);

            return ResponseDto<bool>.GenerateResponse(await postRepository.SaveChangesAsync() > 0)
                .Success(true, HttpStatusCode.OK)
                .Fail("An error occured while updating!", HttpStatusCode.InternalServerError);
        }
    }
}
