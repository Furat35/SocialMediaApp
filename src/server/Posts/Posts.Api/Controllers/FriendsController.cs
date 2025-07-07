using BuildingBlocks.Controllers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Posts.Api.Core.Application.Features.Friends.AcceptFollowRequest;
using Posts.Api.Core.Application.Features.Friends.DeclineFollow;
using Posts.Api.Core.Application.Features.Friends.GetFollowerPosts;
using Posts.Api.Core.Application.Features.Friends.GetFollowersByUserId;
using Posts.Api.Core.Application.Features.Friends.GetFollowRequests;
using Posts.Api.Core.Application.Features.Friends.RemoveFollowRequest;
using Posts.Api.Core.Application.Features.Friends.SendFollowRequestFriend;

namespace Posts.Api.Controllers
{
    [Authorize]
    public class FriendsController(IMediator mediator) : BaseController
    {
        [HttpGet("byUser")]
        public async Task<IActionResult> GetFollowersByUserId([FromQuery] GetFollowersByUserIdQuery request)
        {
            var response = await mediator.Send(request);
            return CreateActionResult(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetFollowerPosts([FromQuery] GetFollowerPostsQuery request)
        {
            var response = await mediator.Send(request);
            return CreateActionResult(response);
        }


        [HttpGet("follow-requests")]
        public async Task<IActionResult> GetFollowRequests([FromQuery] GetFollowRequestsQuery request)
        {
            var response = await mediator.Send(request);
            return Ok(response);
        }


        [HttpPost("add/{userId}")]
        public async Task<IActionResult> SendFollowRequest([FromRoute] SendFollowRequestCommand request)
        {
            var response = await mediator.Send(request);
            return CreateActionResult(response);
        }

        [HttpPost("remove/{userId}")]
        public async Task<IActionResult> RemoveFollowRequest([FromRoute] RemoveFollowRequestCommand request)
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
        public async Task<IActionResult> AcceptFollowRequest([FromRoute] DeclineFollowCommand request)
        {
            var response = await mediator.Send(request);
            return CreateActionResult(response);
        }
    }
}
