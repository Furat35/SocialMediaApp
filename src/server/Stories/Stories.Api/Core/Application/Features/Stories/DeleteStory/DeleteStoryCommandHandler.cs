using BuildingBlocks.Extensions;
using BuildingBlocks.Interfaces.Services;
using BuildingBlocks.Models;
using BuildingBlocks.Models.Constants;
using MediatR;
using Stories.Api.Core.Application.Repositories;
using System.Net;

namespace Stories.Api.Core.Application.Features.Stories.DeleteStory
{
    public class DeleteStoryCommandHandler(
        IStoryRepository storyRepository,
        IHttpContextAccessor httpContext)
        : IRequestHandler<DeleteStoryCommand, ResponseDto<bool>>
    {
        public async Task<ResponseDto<bool>> Handle(DeleteStoryCommand request, CancellationToken cancellationToken)
        {
            var story = await storyRepository.GetByIdAsync(request.StoryId);
            if (story is null)
                return ResponseDto<bool>.Fail(ErrorMessages.NotFound, HttpStatusCode.NotFound);

            if (story.UserId != httpContext.GetUserId())
                return ResponseDto<bool>.Fail(ErrorMessages.Forbidden, HttpStatusCode.Forbidden);

            //fileService.RemoveFile(story.ImagePath);
            storyRepository.Remove(story);

            story.IsValid = false;

            return ResponseDto<bool>.GenerateResponse(await storyRepository.SaveChangesAsync() > 0)
                .Success(true, HttpStatusCode.OK)
                .Fail(ErrorMessages.DeleteError, HttpStatusCode.InternalServerError);

        }
    }
}
