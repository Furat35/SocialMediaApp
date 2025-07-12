using BuildingBlocks.Controllers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Posts.Api.Core.Application.Features.Followers.AcceptFollowRequest;
using Posts.Api.Core.Application.Features.Followers.DeclineFollow;
using Posts.Api.Core.Application.Features.Followers.GetFollowerPosts;
using Posts.Api.Core.Application.Features.Followers.GetFollowersByUserId;
using Posts.Api.Core.Application.Features.Followers.GetFollowRequests;
using Posts.Api.Core.Application.Features.Followers.GetFollowStatus;
using Posts.Api.Core.Application.Features.Followers.Unfollow;
using Posts.Api.Core.Application.Features.Followers.SendFollowRequestFriend;
using Posts.Api.Core.Application.Features.Followers.UnfollowFollower;
using Posts.Api.Core.Application.Features.Followers.CancelFollowRequest;

namespace Posts.Api.Controllers
{
    [Authorize]
    public class FollowersController(IMediator mediator) : BaseController
    {
        [HttpGet("byUser")]
        public async Task<IActionResult> GetFollowersByUserId([FromQuery] GetFollowersByUserIdQuery request)
        {
            var response = await mediator.Send(request);
            return Ok(response);
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetFollowerPosts([FromQuery] GetFollowStatusQuery request)
        {
            var response = await mediator.Send(request);
            return Ok(response);
        }

        [HttpGet()]
        public async Task<IActionResult> GetFollowStatus([FromQuery] GetFollowerPostsQuery request)
        {
            var response = await mediator.Send(request);
            return Ok(response);
        }
        
        [HttpGet("follow-requests")]
        public async Task<IActionResult> GetFollowRequests([FromQuery] GetFollowRequestsQuery request)
        {
            var response = await mediator.Send(request);
            return Ok(response);
        }


        [HttpPost("follow/{userId}")]
        public async Task<IActionResult> SendFollowRequest([FromRoute] SendFollowRequestCommand request)
        {
            var response = await mediator.Send(request);
            return CreateActionResult(response);
        }

        [HttpPost("unfollow/{userId}")]
        public async Task<IActionResult> Unfollow([FromRoute] UnfollowCommand request)
        {
            var response = await mediator.Send(request);
            return CreateActionResult(response);
        }

        [HttpPost("accept/{userId}")]
        public async Task<IActionResult> AcceptFollowRequest([FromRoute] AcceptFollowRequestCommand request)
        {
            var response = await mediator.Send(request);
            return CreateActionResult(response);
        }

        [HttpPost("decline/{userId}")]
        public async Task<IActionResult> DeclineFollowRequest([FromRoute] DeclineFollowCommand request)
        {
            var response = await mediator.Send(request);
            return CreateActionResult(response);
        }

        [HttpPost("cancel/{userId}")]
        public async Task<IActionResult> CancelFollowRequest([FromRoute] CancelFollowRequestCommand request)
        {
            var response = await mediator.Send(request);
            return CreateActionResult(response);
        }
    }
}
