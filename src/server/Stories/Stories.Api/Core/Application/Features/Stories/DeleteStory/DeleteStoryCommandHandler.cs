using BuildingBlocks.Extensions;
using BuildingBlocks.Helpers;
using BuildingBlocks.Models;
using MediatR;
using Stories.Api.Core.Application.Repositories;
using Stories.Api.Core.Domain.Entities;
using System.Net;

namespace Stories.Api.Core.Application.Features.Stories.DeleteStory
{
    public class DeleteStoryCommandHandler(
        IStoryRepository storyRepository,
        IHttpContextAccessor httpContext)
        : BaseHandler<IStoryRepository, Story>(storyRepository),
            IRequestHandler<DeleteStoryCommand, ResponseDto<bool>>
    {
        public async Task<ResponseDto<bool>> Handle(DeleteStoryCommand request, CancellationToken cancellationToken)
        {
            var story = await storyRepository
                .GetFirstAsync(_ => _.UserId == httpContext.GetUserId() && _.Id == request.StoryId && _.IsValid);
            if (story is null) return HttpStatusCode.NotFound.ToResponse<bool>();

            //fileService.RemoveFile(story.ImagePath);
            //storyRepository.Remove(story);
            story.IsValid = false;

            return await SaveChangesAsync();
        }
    }
}
