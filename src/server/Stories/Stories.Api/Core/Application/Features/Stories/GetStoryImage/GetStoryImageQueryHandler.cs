using MediatR;
using Microsoft.AspNetCore.StaticFiles;
using Stories.Api.Core.Application.Repositories;

namespace Stories.Api.Core.Application.Features.Stories.GetStoryImage
{
    public class GetStoryImageQueryHandler(IStoryRepository storyRepository)
        : IRequestHandler<GetStoryImageQuery, (byte[] image, string fileType)>
    {
        public async Task<(byte[] image, string fileType)> Handle(GetStoryImageQuery request, CancellationToken cancellationToken)
        {
            var story = await storyRepository.GetByIdAsync(request.StoryId);

            if (!story.IsValid)
                throw new BadHttpRequestException("Story is deleted!");

            //if (story is not null && !await followerRepository.ActiveUserHasAccessToGivenUser(story.UserId))
            //{
            //    throw new BadHttpRequestException("You do not have permission to access this user's stories.");
            //}

            if (!File.Exists(story.ImagePath))
                throw new Exception("File doesn't exist");

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(story.ImagePath, out var contentType))
            {
                contentType = "application/octet-stream"; // fallback for unknown types
            }


            return (File.ReadAllBytes(story.ImagePath), contentType);
        }
    }
}
