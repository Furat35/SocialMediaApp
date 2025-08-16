using BuildingBlocks.Extensions;
using BuildingBlocks.Models;
using MediatR;
using Posts.Api.Core.Application.Repositories;
using Posts.Api.Core.Application.Services;
using System.Net;

namespace Posts.Api.Core.Application.Features.Stories.DeleteStory
{
    public class DeleteStoryCommandHandler(IStoryRepository storyRepository, IHttpContextAccessor httpContext, IImageService imageService)
        : IRequestHandler<DeleteStoryCommand, ResponseDto<bool>>
    {
        public async Task<ResponseDto<bool>> Handle(DeleteStoryCommand request, CancellationToken cancellationToken)
        {
            var story = await storyRepository.GetByIdAsync(request.StoryId);
            if (story is null)
                return ResponseDto<bool>.Fail("Story not found", HttpStatusCode.NotFound);

            if (story.UserId != httpContext.GetUserId())
                return ResponseDto<bool>.Fail("You do not have permission to delete this story", HttpStatusCode.Forbidden);

            //imageService.RemoveImage(story.ImagePath);
            //storyRepository.Remove(story);

            story.IsValid = false;

            return ResponseDto<bool>.GenerateResponse(await storyRepository.SaveChangesAsync() > 0)
                .Success(true, HttpStatusCode.OK)
                .Fail("An error occured while deleting story!", HttpStatusCode.InternalServerError);

        }
    }
}
