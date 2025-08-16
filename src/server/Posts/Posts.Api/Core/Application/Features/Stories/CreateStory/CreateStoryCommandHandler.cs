using AutoMapper;
using BuildingBlocks.Extensions;
using BuildingBlocks.Models;
using MediatR;
using Posts.Api.Core.Application.Dtos.Stories;
using Posts.Api.Core.Application.Repositories;
using Posts.Api.Core.Application.Services;
using Posts.Api.Core.Domain.Entities;
using System.Net;

namespace Posts.Api.Core.Application.Features.Stories.CreateStory
{
    public class CreateStoryCommandHandler(IStoryRepository storyRepository, IHttpContextAccessor httpContext, 
         IMapper mapper, IImageService imageService)
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
                path = await imageService.SaveImageAsync(file, "images/users/stories");
                story.ImagePath = path;
                await storyRepository.AddAsync(story);
                result = await storyRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                imageService.RemoveImage(path);
                throw ex;
            }

            return ResponseDto<StoryListDto>.GenerateResponse(result > 0)
                .Success(mapper.Map<StoryListDto>(story), HttpStatusCode.OK)
                .Fail("An error occured while adding comment!", HttpStatusCode.InternalServerError);
        }
    }
}
