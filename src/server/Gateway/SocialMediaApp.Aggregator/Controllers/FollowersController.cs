using BuildingBlocks.Extensions;
using BuildingBlocks.Models;
using Microsoft.AspNetCore.Mvc;
using SocialMediaApp.Aggregator.Models.Followers;
using SocialMediaApp.Aggregator.Services;

namespace SocialMediaApp.Aggregator.Controllers
{
    public class FollowersController(IHttpContextAccessor httpContext,
        IUserService userService, IFollowerService followerService)
        : AggregatedBaseController
    {

        [HttpGet("follow-requests")]
        public async Task<ActionResult<PaginationResponseModel<FollowerListDto>>> GetFollowRequests([FromQuery] PaginationRequestModel request)
        {
            var followResponse = await followerService.GetFollowRequestsAsync(request);

            var followRequestUserIds = followResponse.Data.Select(p => p.RequestingUserId).Distinct().ToList();
            var followRequestResult = await userService.GetUsersWithGivenIdsAsync(followRequestUserIds);

            followResponse.Data.ForEach(p =>
            {
                p.User = followRequestResult.Data.First(_ => _.Id == p.RequestingUserId);
            });

            return Ok(followResponse);
        }

        [HttpGet("byUser")]
        public async Task<ActionResult<PaginationResponseModel<FollowerListDto>>> GetFollowersByUserId([FromQuery] int userId, [FromQuery] int status, [FromQuery] PaginationRequestModel request)
        {
            var followersResponse = await followerService.GetFollowersByUserIdAsync(request, userId, status);

            var requestingUserIds = followersResponse.Data.Select(p => p.RequestingUserId).Except([userId]);
            var respondingUserIds = followersResponse.Data.Select(p => p.RespondingUserId).Except([userId]);
            var followRequestUserIds = requestingUserIds.Concat(respondingUserIds).Distinct().ToList();
            var followRequestResult = await userService.GetUsersWithGivenIdsAsync(followRequestUserIds);

            followersResponse.Data.ForEach(p =>
                    p.User = followRequestResult.Data.FirstOrDefault(_ => _.Id == p.RequestingUserId) ??
                    followRequestResult.Data.FirstOrDefault(_ => _.Id == p.RespondingUserId));

            return Ok(followersResponse);
        }

        [HttpGet]
        public async Task<ActionResult<PaginationResponseModel<FollowerListDto>>> GetFollowerPosts([FromQuery] int userId, [FromQuery] int status, [FromQuery] PaginationRequestModel request)
        {
            var followersResponse = await followerService.GetFollowersByUserIdAsync(request, userId, status);

            var requestingUserIds = followersResponse.Data.Select(p => p.RequestingUserId).Except([userId]);
            var respondingUserIds = followersResponse.Data.Select(p => p.RespondingUserId).Except([userId]);
            var followRequestUserIds = requestingUserIds.Concat(respondingUserIds).Distinct().ToList();
            var followRequestResult = await userService.GetUsersWithGivenIdsAsync(followRequestUserIds);

            followersResponse.Data.ForEach(p =>
                    p.User = followRequestResult.Data.FirstOrDefault(_ => _.Id == p.RequestingUserId) ??
                    followRequestResult.Data.FirstOrDefault(_ => _.Id == p.RespondingUserId));

            return Ok(followersResponse);
        }

        [HttpGet("search")]
        public async Task<ActionResult<PaginationResponseModel<FollowerListDto>>> SearchFollowers([FromQuery] string searchKey, [FromQuery] PaginationRequestModel request)
        {
            var userResponse = await userService.SearchUserAsync(searchKey);

            var followerResponse = await followerService.SearchFollower(userResponse.Data, request);

            var followerUserIds = followerResponse.Data.Select(_ => _.RespondingUserId == httpContext.GetUserId() ? _.RequestingUserId : _.RespondingUserId).ToList();
            var users = await userService.GetUsersWithGivenIdsAsync(followerUserIds);


            followerResponse.Data.ForEach(p =>
            {
                p.User = users.Data.First(_ => _.Id == (p.RespondingUserId == httpContext.GetUserId() ? p.RequestingUserId : p.RespondingUserId));
            });

            return Ok(followerResponse);
        }
    }
}
