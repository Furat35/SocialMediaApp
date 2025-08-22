using AutoMapper;
using BuildingBlocks.Extensions;
using BuildingBlocks.Interfaces.Services;
using BuildingBlocks.Models;
using BuildingBlocks.Models.Constants;
using MediatR;
using Stories.Api.Core.Application.Dtos.Stories;
using Stories.Api.Core.Application.Repositories;
using Stories.Api.Core.Domain.Entities;
using System.Net;

namespace Stories.Api.Core.Application.Features.Stories.CreateStory
{
    public class CreateStoryCommandHandler(
        IStoryRepository storyRepository,
        IHttpContextAccessor httpContext,
        IMapper mapper, 
        IFileService imageService)
        : IRequestHandler<CreateStoryCommand, ResponseDto<StoryListDto>>
    {
        public async Task<ResponseDto<StoryListDto>> Handle(CreateStoryCommand request, CancellationToken cancellationToken)
        {
            var story = new Story
            {
                UserId = httpContext.GetUserId(),
                ImagePath = null,
            };
            string path = null;
            int result = 0;
            try
            {
                var file = httpContext.HttpContext.Request.Form.Files.FirstOrDefault();
                path = await imageService.SaveFileAsync(file, "images/users/stories");
                story.ImagePath = path;
                await storyRepository.AddAsync(story);
                result = await storyRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                imageService.RemoveFile(path);
                throw;
            }

            return ResponseDto<StoryListDto>.GenerateResponse(result > 0)
                .Success(mapper.Map<StoryListDto>(story), HttpStatusCode.OK)
                .Fail(ErrorMessages.CreateError, HttpStatusCode.InternalServerError);
        }
    }
}
