using MediatR;
using Microsoft.AspNetCore.StaticFiles;
using Posts.Api.Core.Application.Repositories;

namespace Posts.Api.Core.Application.Features.Posts.GetPostImage
{
    public class GetPostImageQueryHandler(IPostRepository postRepository)
        : IRequestHandler<GetPostImageQuery, (byte[] image, string fileType)>
    {
        public async Task<(byte[] image, string fileType)> Handle(GetPostImageQuery request, CancellationToken cancellationToken)
        {
            var post = await postRepository.GetByIdAsync(request.PostId);
            if (!File.Exists(post.ImagePath))
                throw new Exception("File doesn't exist");

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(post.ImagePath, out var contentType))
            {
                contentType = "application/octet-stream"; // fallback for unknown types
            }


            return (File.ReadAllBytes(post.ImagePath), contentType);
        }
    }
}
