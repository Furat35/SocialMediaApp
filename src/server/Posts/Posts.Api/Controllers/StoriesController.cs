using BuildingBlocks.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Posts.Api.Core.Application.Features.Posts.GetPostImage;
using Posts.Api.Core.Application.Features.Stories.CreateStory;
using Posts.Api.Core.Application.Features.Stories.DeleteStory;
using Posts.Api.Core.Application.Features.Stories.GetStories;
using Posts.Api.Core.Application.Features.Stories.GetStoryByUserId;
using Posts.Api.Core.Application.Features.Stories.GetStoryImage;

namespace Posts.Api.Controllers
{
    public class StoriesController(IMediator mediator) : BaseController
    {
        [HttpPost]
        public async Task<IActionResult> Create([FromQuery] CreateStoryCommand request)
        {
            var result = await mediator.Send(request);
            return CreateActionResult(result);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] DeleteStoryCommand request)
        {
            var result = await mediator.Send(request);
            return CreateActionResult(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetStories([FromQuery] GetStoriesQuery request)
        {
            var result = await mediator.Send(request);
            return Ok(result);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetById([FromRoute] GetStoryByUserIdQuery request)
        {
            var result = await mediator.Send(request);
            return CreateActionResult(result);
        }

        [HttpGet("image")]
        public async Task<IActionResult> GetStoryImage([FromQuery] GetStoryImageQuery request)
        {
            var response = await mediator.Send(request);
            return File(response.image, response.fileType);
        }
    }
}
