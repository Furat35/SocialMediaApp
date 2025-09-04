using BuildingBlocks.Controllers;
using BuildingBlocks.Models;
using IdentityServer.Api.Business.Dtos.Followers;
using IdentityServer.Api.Business.Interfaces;
using IdentityServer.Api.Core.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Api.Controllers
{
    [Authorize]
    public class FollowersController(IFollowerService followerService) : BaseController
    {
        [HttpGet("byUser")]
        public async Task<ActionResult<PaginationResponseModel<FollowerListDto>>> GetFollowersByUserId([FromQuery] int userId, [FromQuery] PaginationRequestModel request, [FromQuery] FollowStatus status)
        {
            var response = await followerService.GetFollowersByUserId(userId, status, request);
            return Ok(response);
        }

        [HttpGet("hasAccessTo")]
        public async Task<ActionResult<bool>> UserHasAccessToGivenUser([FromQuery] int userId)
        {
            var response = await followerService.ActiveUserHasAccessToGivenUser(userId);
            return Ok(response);
        }

        [HttpGet("status")]
        public async Task<ActionResult<ResponseDto<FollowerListDto>>> GetFollowStatus([FromQuery] int userId)
        {
            var response = await followerService.GetFollowStatus(userId);
            return Ok(response);
        }

        [HttpGet("ids")]
        public async Task<ActionResult<ResponseDto<List<int>>>> GetFollowerIds()
        {
            var response = await followerService.GetFollowerIds();
            return Ok(response);
        }

        //[HttpGet]
        //public async Task<IActionResult> GetFollowStatus([FromQuery] int userId)
        //{
        //    var response = await followerService.GetFollowStatus(userId);
        //    return Ok(response);
        //}

        [HttpGet("count")]
        public async Task<ActionResult<int>> GetFollowerCount([FromQuery] int userId)
        {
            var response = await followerService.GetFollowersCount(userId);
            return Ok(response);
        }

        [HttpGet("follow-requests")]
        public async Task<ActionResult<PaginationResponseModel<FollowerListDto>>> GetFollowRequests([FromQuery] PaginationRequestModel request)
        {
            var response = await followerService.GetFollowRequests(request);
            return Ok(response);
        }


        [HttpPost("search")]
        public async Task<ActionResult<PaginationResponseModel<FollowerListDto>>> GetFollowersByUserIds([FromQuery] PaginationRequestModel request, [FromBody] List<int> userIds)
        {
            var response = await followerService.GetFollowersByUserIds(request, userIds);
            return Ok(response);
        }

        [HttpPost("follow/{userId}")]
        public async Task<ActionResult<ResponseDto<bool>>> SendFollowRequest([FromRoute] int userId)
        {
            var response = await followerService.SendFollowRequest(userId);
            return CreateActionResult(response);
        }

        [HttpPost("unfollow/{userId}")]
        public async Task<ActionResult<ResponseDto<bool>>> Unfollow([FromRoute] int userId)
        {
            var response = await followerService.Unfollow(userId);
            return CreateActionResult(response);
        }

        [HttpPost("accept/{userId}")]
        public async Task<ActionResult<ResponseDto<bool>>> AcceptFollowRequest([FromRoute] int userId)
        {
            var response = await followerService.AcceptFollowRequest(userId);
            return CreateActionResult(response);
        }

        [HttpPost("decline/{userId}")]
        public async Task<ActionResult<ResponseDto<bool>>> DeclineFollowRequest([FromRoute] int userId)
        {
            var response = await followerService.DeclineFollowRequest(userId);
            return CreateActionResult(response);
        }

        [HttpPost("cancel/{userId}")]
        public async Task<ActionResult<ResponseDto<bool>>> CancelFollowRequest([FromRoute] int userId)
        {
            var response = await followerService.CancelFollowRequest(userId);
            return CreateActionResult(response);
        }

        [HttpPost("ban/{userId}")]
        public async Task<ActionResult<ResponseDto<bool>>> BanFollower([FromRoute] int userId)
        {
            var response = await followerService.BanFollower(userId);
            return CreateActionResult(response);
        }

        [HttpPost("removeBan/{userId}")]
        public async Task<ActionResult<ResponseDto<bool>>> RemoveBan([FromRoute] int userId)
        {
            var response = await followerService.RemoveBan(userId);
            return CreateActionResult(response);
        }
    }
}
