using BuildingBlocks.Controllers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Posts.Api.Core.Application.Features.Friends.AcceptFollowRequest;
using Posts.Api.Core.Application.Features.Friends.GetFollowerPosts;
using Posts.Api.Core.Application.Features.Friends.RemoveFollowRequest;
using Posts.Api.Core.Application.Features.Friends.SendFollowRequestFriend;

namespace Posts.Api.Controllers
{
    [Authorize]
    public class FriendsController(IMediator mediator) : BaseController
    {
        //[HttpGet]
        //public async Task<IActionResult> GetFollowersByUserId([FromQuery] GetFollowersByUserIdCommand request)
        //{
        //    var response = await mediator.Send(request);
        //    return CreateActionResult(response);
        //}

        [HttpGet]
        public async Task<IActionResult> GetFollowerPosts([FromQuery] GetFollowerPostsQuery request)
        {
            var response = await mediator.Send(request);
            return CreateActionResult(response);
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
    }
}
