using BuildingBlocks.Extensions;
using BuildingBlocks.Models;
using Consul;
using Microsoft.AspNetCore.Mvc;
using SocialMediaApp.Aggregator.helpers;
using SocialMediaApp.Aggregator.Models.Followers;
using SocialMediaApp.Aggregator.Models.Users;

namespace SocialMediaApp.Aggregator.Controllers
{
    public class FollowersController(IHttpClientFactory httpClientFactory, IConsulClient consulClient, IHttpContextAccessor httpContext) : AggregatedBaseController
    {
        private readonly HttpClient? _httpClient = httpClientFactory.CreateClient("default");

        [HttpGet("follow-requests")]
        public async Task<IActionResult> GetFollowRequests([FromQuery] PaginationRequestModel request)
        {
            var postApiServiceUrl = await consulClient.ResolveServiceUrl("posts.api");
            var followResponse = await _httpClient.GetFromJsonAsync<PaginationResponseModel<FollowerListDto>>($"{postApiServiceUrl}/api/followers/follow-requests?page={request.Page}&pageSize={request.PageSize}");

            var followRequestUserIds = followResponse.Data.Select(p => p.RequestingUserId).Distinct().ToList();
            var followRequestResult = await GetUsersWithGivenIdsAsync(followRequestUserIds);

            followResponse.Data.ForEach(p =>
            {
                p.User = followRequestResult.First(_ => _.Id == p.RequestingUserId);
            });

            return Ok(followResponse);
        }

        [HttpGet("byUser")]
        public async Task<IActionResult> GetFollowersByUserId([FromQuery] int userId, [FromQuery] int status, [FromQuery] PaginationRequestModel request)
        {
            var httpClient = httpClientFactory.CreateClient("default");

            var postApiServiceUrl = await consulClient.ResolveServiceUrl("posts.api");
            var followersResponse = await httpClient.GetFromJsonAsync<PaginationResponseModel<FollowerListDto>>($"{postApiServiceUrl}/api/followers/byuser?status={status}&userId={userId}&page={request.Page}&pageSize={request.PageSize}");

            var requestingUserIds = followersResponse.Data.Select(p => p.RequestingUserId).Except([userId]);
            var respondingUserIds = followersResponse.Data.Select(p => p.RespondingUserId).Except([userId]);
            var followRequestUserIds = requestingUserIds.Concat(respondingUserIds).Distinct().ToList();
            var followRequestResult = await GetUsersWithGivenIdsAsync(followRequestUserIds);

            followersResponse.Data.ForEach(p =>
                    p.User = followRequestResult.FirstOrDefault(_ => _.Id == p.RequestingUserId) ??
                    followRequestResult.FirstOrDefault(_ => _.Id == p.RespondingUserId));

            return Ok(followersResponse);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchFollowers([FromQuery] string searchKey, [FromQuery] PaginationRequestModel request)
        {
            var identityServiceUrl = await consulClient.ResolveServiceUrl("identityserver.api");
            var userResponse = await _httpClient.GetFromJsonAsync<ResponseDto<List<int>>>($"{identityServiceUrl}/api/users/search?searchKey={searchKey}");

            var postServiceUrl = await consulClient.ResolveServiceUrl("posts.api");
            var postResponse = await _httpClient.PostAsJsonAsync($"{postServiceUrl}/api/followers/search?page={request.Page}&pageSize={request.PageSize}",
                new { UserIds = userResponse.Data, request.Page, request.PageSize });

            var followerResponse = await postResponse.Content.ReadFromJsonAsync<PaginationResponseModel<FollowerListDto>>();
            var followerUserIds = followerResponse.Data.Select(_ => _.RespondingUserId == httpContext.GetUserId() ? _.RequestingUserId : _.RespondingUserId).ToList();
            var users = await GetUsersWithGivenIdsAsync(followerUserIds);


            followerResponse.Data.ForEach(p =>
            {
                p.User = users.First(_ => _.Id == (p.RespondingUserId == httpContext.GetUserId() ? p.RequestingUserId : p.RespondingUserId));
            });

            return Ok(followerResponse);
        }

        private async Task<List<UserListDto>> GetUsersWithGivenIdsAsync(List<int> userIds)
        {
            var identityServiceUrl = await consulClient.ResolveServiceUrl("identityserver.api");
            var request = await _httpClient.PostAsJsonAsync($"{identityServiceUrl}/api/users", userIds);

            var userResponse = await request.Content.ReadFromJsonAsync<ResponseDto<List<UserListDto>>>();

            return userResponse.Data;
        }
    }
}
