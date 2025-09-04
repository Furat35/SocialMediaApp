using BuildingBlocks.Controllers;
using BuildingBlocks.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Posts.Api.Core.Application.Dtos.Posts;
using Posts.Api.Core.Application.Features.Posts.CreatePost;
using Posts.Api.Core.Application.Features.Posts.CreatePostComment;
using Posts.Api.Core.Application.Features.Posts.DeletePost;
using Posts.Api.Core.Application.Features.Posts.GetFollowerPosts;
using Posts.Api.Core.Application.Features.Posts.GetPostImage;
using Posts.Api.Core.Application.Features.Posts.GetPostsByUserId;
using Posts.Api.Core.Application.Features.Posts.LikePost;
using Posts.Api.Core.Application.Features.Posts.RemovePostComment;
using Posts.Api.Core.Application.Features.Posts.UnlikePost;
using Posts.Api.Core.Application.Features.Posts.UpdatePost;

namespace Posts.Api.Controllers
{
    [Authorize]
    public class PostsController(IMediator mediatr) : BaseController
    {
        [HttpGet]
        public async Task<ActionResult<PaginationResponseModel<PostListDto>>> GetPostsByUserId([FromQuery] GetPostsByUserIdQuery request)
        {
            var response = await mediatr.Send(request);
            return Ok(response);
        }

        [HttpGet("follower-posts")]
        public async Task<ActionResult<PaginationResponseModel<PostListDto>>> GetFollowerPosts([FromQuery] GetFollowerPostsQuery request)
        {
            var response = await mediatr.Send(request);
            return Ok(response);
        }

        [HttpGet("image")]
        public async Task<IActionResult> GetPostImage([FromQuery] GetPostImageQuery request)
        {
            var response = await mediatr.Send(request);
            return File(response.image, response.fileType);
        }

        [HttpPost]
        public async Task<ActionResult<ResponseDto<CreatePostCommandResponse>>> CreatePost([FromForm] CreatePostCommandRequest request)
        {
            var response = await mediatr.Send(request);
            return CreateActionResult(response);
        }

        [HttpPost("like")]
        public async Task<ActionResult<ResponseDto<bool>>> LikePost([FromQuery] LikePostCommand request)
        {
            var response = await mediatr.Send(request);
            return CreateActionResult(response);
        }

        [HttpPost("unlike")]
        public async Task<ActionResult<ResponseDto<bool>>> UnlikePost([FromQuery] UnlikePostCommand request)
        {
            var response = await mediatr.Send(request);
            return CreateActionResult(response);
        }

        [HttpPost("add-comment")]
        public async Task<ActionResult<ResponseDto<bool>>> AddPostComment([FromQuery] CreatePostCommentCommand request)
        {
            var response = await mediatr.Send(request);
            return CreateActionResult(response);
        }

        [HttpPost("remove-comment")]
        public async Task<ActionResult<ResponseDto<bool>>> RemovePostComment([FromQuery] RemovePostCommentCommand request)
        {
            var response = await mediatr.Send(request);
            return CreateActionResult(response);
        }

        [HttpDelete]
        public async Task<ActionResult<ResponseDto<bool>>> DeletePost([FromQuery] DeletePostCommand request)
        {
            var response = await mediatr.Send(request);
            return CreateActionResult(response);
        }

        [HttpPut]
        public async Task<ActionResult<ResponseDto<bool>>> UpdatePost(UpdatePostCommand request)
        {
            var response = await mediatr.Send(request);
            return CreateActionResult(response);
        }
    }
}
