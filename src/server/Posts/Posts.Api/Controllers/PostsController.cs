using BuildingBlocks.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Posts.Api.Core.Application.Features.Posts.CreatePost;
using Posts.Api.Core.Application.Features.Posts.DeletePost;
using Posts.Api.Core.Application.Features.Posts.GetPostById;
using Posts.Api.Core.Application.Features.Posts.GetPostsByUserId;
using Posts.Api.Core.Application.Features.Posts.UpdatePost;

namespace Posts.Api.Controllers
{
    public class PostsController(IMediator mediatr) : BaseController
    {
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPostById(GetPostByIdQuery request)
        {
            var response = await mediatr.Send(request);
            return CreateActionResult(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetPosts([FromQuery] GetPostsByUserIdQuery request)
        {
            var response = await mediatr.Send(request);
            return CreateActionResult(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost(CreatePostCommandRequest request)
        {
            var response = await mediatr.Send(request);
            return CreateActionResult(response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePost(UpdatePostCommand request)
        {
            var response = await mediatr.Send(request);
            return CreateActionResult(response);
        }

        [HttpDelete]
        public async Task<IActionResult> DeletePost(DeletePostCommand request)
        {
            var response = await mediatr.Send(request);
            return CreateActionResult(response);
        }
    }
}
