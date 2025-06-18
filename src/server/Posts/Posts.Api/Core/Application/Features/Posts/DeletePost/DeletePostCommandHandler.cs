using BuildingBlocks.Models;
using MediatR;
using Posts.Api.Core.Application.Repositories;
using System.Net;

namespace Posts.Api.Core.Application.Features.Posts.DeletePost
{
    public class DeletePostCommandHandler(IPostRepository postRepository) : IRequestHandler<DeletePostCommand, ResponseDto<bool>>
    {
        public async Task<ResponseDto<bool>> Handle(DeletePostCommand request, CancellationToken cancellationToken)
        {
            var post = await postRepository.GetByIdAsync(request.Id);
            if (post == null) return ResponseDto<bool>.Fail("Post not found", System.Net.HttpStatusCode.NotFound);

            post.IsValid = false;
            postRepository.Update(post);

            return ResponseDto<bool>.GenerateResponse(await postRepository.SaveChangesAsync() > 0)
                .Success(true, HttpStatusCode.OK)
                .Fail("An error occured while deleting!", HttpStatusCode.InternalServerError);
        }
    }
}
