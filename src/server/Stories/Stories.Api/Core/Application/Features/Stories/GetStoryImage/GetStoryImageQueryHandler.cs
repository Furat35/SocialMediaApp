using BuildingBlocks.Models.Constants;
using MediatR;
using Microsoft.AspNetCore.StaticFiles;
using Stories.Api.Core.Application.Repositories;
using Stories.Api.ExternalServices;

namespace Stories.Api.Core.Application.Features.Stories.GetStoryImage
{
    public class GetStoryImageQueryHandler(
        IStoryRepository storyRepository,
        IFollowerService followerService)
        : IRequestHandler<GetStoryImageQuery, (byte[] image, string fileType)>
    {
        public async Task<(byte[] image, string fileType)> Handle(GetStoryImageQuery request, CancellationToken cancellationToken)
        {
            var story = await storyRepository.GetByIdAsync(request.StoryId);

            if (story is null || !story.IsValid)
                throw new BadHttpRequestException(ErrorMessages.BadRequest);

            if (story.UserId != request.FollowerId) throw new BadHttpRequestException(ErrorMessages.Forbidden);

            if (story is not null && !await followerService.IsFollowing(story.UserId))
                throw new BadHttpRequestException(ErrorMessages.Forbidden);

            if (!File.Exists(story.ImagePath))
                throw new Exception(ErrorMessages.NotFound);

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(story.ImagePath, out var contentType))
            {
                contentType = "application/octet-stream"; // fallback for unknown types
            }


            return (File.ReadAllBytes(story.ImagePath), contentType);
        }
    }
}
