using BuildingBlocks.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Posts.Api.Core.Application.Features.Posts.CreatePost;
using Posts.Api.Core.Application.Repositories;
using Posts.Api.Core.Application.Services;
using Posts.Api.Core.Domain.Entities;
using Posts.Api.Infrastructure.Repositories;
using Posts.Api.Infrastructure.Services;
using System.IO;
using System.Net;

namespace Posts.Api.Core.Application.Features.Stories.CreateStory
{
    //public class CreateStoryCommandHandler(IStoryRepository storyRepository, IHttpContextAccessor httpContext, IImageService imageService) 
    //    : IRequestHandler<CreateStoryCommand, ResponseDto<bool>>
    //{
    //    public async Task<ResponseDto<bool>> Handle(CreateStoryCommand request, CancellationToken cancellationToken)
    //    {
    //        var story = new Story
    //        {
    //            UserId = httpContext.HttpContext.User.FindFirst("id")?.Value,
    //            ImagePath = null,
    //        };
    //        string path = null;
    //        int result = 0;
    //        try
    //        {
    //            var file = httpContext.HttpContext.Request.Form.Files.FirstOrDefault();
    //            path = await imageService.SaveImageAsync(file, "images/users/stories");
    //            story.ImagePath = path;
    //            await storyRepository.AddAsync(story);
    //            result = await storyRepository.SaveChangesAsync();
    //        }
    //        catch (Exception ex)
    //        {
    //            imageService.RemoveImage(path);
    //            throw ex;
    //        }

    //        var response = new CreatePostCommandResponse
    //        {
    //            Id = story?.Id,
    //            Success = result > 0
    //        };

    //        return ResponseDto<bool>.Success(response, HttpStatusCode.OK);
    //    }
    //}
}
