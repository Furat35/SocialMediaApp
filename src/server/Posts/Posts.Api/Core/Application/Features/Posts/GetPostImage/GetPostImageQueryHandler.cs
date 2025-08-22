using BuildingBlocks.Models.Constants;
using MediatR;
using Microsoft.AspNetCore.StaticFiles;
using Posts.Api.Core.Application.Repositories;
using Posts.Api.ExternalServices;

namespace Posts.Api.Core.Application.Features.Posts.GetPostImage
{
    public class GetStoryImageQueryHandler(IPostRepository postRepository)
        : IRequestHandler<GetPostImageQuery, (byte[] image, string fileType)>
    {
        public async Task<(byte[] image, string fileType)> Handle(GetPostImageQuery request, CancellationToken cancellationToken)
        {
            var post = await postRepository.GetByIdAsync(request.PostId);
            if (post is null)
                throw new BadHttpRequestException(ErrorMessages.BadRequest);
            if (post.UserId != request.FollowerId) throw new BadHttpRequestException(ErrorMessages.Forbidden);

            if (!File.Exists(post.ImagePath))
                throw new Exception(ErrorMessages.FileNotFound);

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(post.ImagePath, out var contentType))
            {
                contentType = "application/octet-stream"; // fallback for unknown types
            }


            return (File.ReadAllBytes(post.ImagePath), contentType);
        }
    }
}
