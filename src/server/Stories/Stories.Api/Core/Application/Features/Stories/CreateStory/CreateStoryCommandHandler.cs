using BuildingBlocks.Extensions;
using BuildingBlocks.Interfaces.Services;
using BuildingBlocks.Models;
using BuildingBlocks.Models.Constants;
using MediatR;
using Stories.Api.Core.Application.Dtos.Stories;
using Stories.Api.Core.Application.Repositories;
using Stories.Api.Core.Domain.Entities;

namespace Stories.Api.Core.Application.Features.Stories.CreateStory
{
    public class CreateStoryCommandHandler(
        IStoryRepository storyRepository,
        IHttpContextAccessor httpContext,
        IFileService fileService)
        : BaseHandler<IStoryRepository, Story>(storyRepository),
            IRequestHandler<CreateStoryCommand, ResponseDto<StoryListDto>>
    {
        public async Task<ResponseDto<StoryListDto>> Handle(CreateStoryCommand request, CancellationToken cancellationToken)
        {
            var story = new Story { UserId = httpContext.GetUserId(), ImagePath = null };
            string path = null;
            int result = 0;
            try
            {
                var file = httpContext.GetFirstFormFile();
                path = await fileService.SaveFileAsync(file, "images/users/stories");
                story.ImagePath = path;
                await storyRepository.AddAsync(story);
                result = await storyRepository.SaveChangesAsync();
            }
            catch
            {
                fileService.RemoveFile(path);
                throw;
            }

            return result != 0
                ? ReturnSuccess<StoryListDto>()
                : ReturnFail<StoryListDto>(null, ErrorMessages.SaveChangesError);
        }
    }
}
