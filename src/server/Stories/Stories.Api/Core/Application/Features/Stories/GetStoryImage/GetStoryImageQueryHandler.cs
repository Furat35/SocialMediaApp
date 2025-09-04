using BuildingBlocks.Models;
using BuildingBlocks.Models.Constants;
using MediatR;
using Microsoft.AspNetCore.StaticFiles;
using Stories.Api.Core.Application.Repositories;
using Stories.Api.Core.Domain.Entities;

namespace Stories.Api.Core.Application.Features.Stories.GetStoryImage
{
    public class GetStoryImageQueryHandler(
        IStoryRepository storyRepository)
        : BaseHandler<IStoryRepository, Story>(storyRepository),
            IRequestHandler<GetStoryImageQuery, (byte[] image, string fileType)>
    {
        public async Task<(byte[] image, string fileType)> Handle(GetStoryImageQuery request, CancellationToken cancellationToken)
        {
            var story = await storyRepository
                .GetFirstAsync(_ => _.Id == request.StoryId && _.UserId == request.FollowerId && _.IsValid);

            if (story is null)
                throw new BadHttpRequestException(ErrorMessages.NotFound);

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
