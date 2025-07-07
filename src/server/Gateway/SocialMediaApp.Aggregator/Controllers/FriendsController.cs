using BuildingBlocks.Controllers;
using BuildingBlocks.Models;
using Consul;
using Microsoft.AspNetCore.Mvc;
using SocialMediaApp.Aggregator.helpers;
using SocialMediaApp.Aggregator.Models.Friends;
using SocialMediaApp.Aggregator.Models.Users;

namespace SocialMediaApp.Aggregator.Controllers
{
    public class FriendsController(IHttpClientFactory httpClientFactory, IConsulClient consulClient) : AggregatedBaseController
    {
        private readonly HttpClient? _httpClient = httpClientFactory.CreateClient("default");

        [HttpGet("follow-requests")]
        public async Task<IActionResult> GetFollowRequests([FromQuery] PaginationRequestModel request)
        {
            var httpClient = httpClientFactory.CreateClient("default");

            var friendRequestsServiceUrl = await consulClient.ResolveServiceUrl("posts.api");
            var friendRequestsResponse = await httpClient.GetFromJsonAsync<PaginationResponseModel<FollowRequestsDto>>($"{friendRequestsServiceUrl}/api/friends/follow-requests?page={request.Page}&pageSize={request.PageSize}");

            var followRequestUserIds = friendRequestsResponse.Data.Select(p => p.RequestingUserId).Distinct().ToList();
            var followRequestResult = await GetUsersWithGivenIdsAsync(followRequestUserIds);

            friendRequestsResponse.Data.ForEach(p =>
            {
                p.User = followRequestResult.First(_ => _.Id == p.RequestingUserId);
            });

            return Ok(friendRequestsResponse);
        }

        [HttpGet("byUser")]
        public async Task<IActionResult> GetFollowersByUserId([FromQuery] int userId, [FromQuery] PaginationRequestModel request)
        {
            var httpClient = httpClientFactory.CreateClient("default");

            var friendsServiceUrl = await consulClient.ResolveServiceUrl("posts.api");
            var friendsServiceResponse = await httpClient.GetFromJsonAsync<ResponseDto<PaginationResponseModel<FriendListDto>>>($"{friendsServiceUrl}/api/friends/byuser?userId={userId}&page={request.Page}&pageSize={request.PageSize}");

            var followRequestUserIds = friendsServiceResponse.Data.Data.Select(p => p.UserId).Distinct().ToList();
            var followRequestResult = await GetUsersWithGivenIdsAsync(followRequestUserIds);

            friendsServiceResponse.Data.Data.ForEach(p =>
            {
                p.User = followRequestResult.First(_ => _.Id == p.UserId);
            });

            return Ok(friendsServiceResponse);
        }

        public async Task<List<UserListDto>> GetUsersWithGivenIdsAsync(List<int> userIds)
        {
            var identityServiceUrl = await consulClient.ResolveServiceUrl("identityserver.api");
            var request = await _httpClient.PostAsJsonAsync($"{identityServiceUrl}/api/users", userIds);

            var userResponse = await request.Content.ReadFromJsonAsync<ResponseDto<List<UserListDto>>>();

            return userResponse.Data;
        }
    }
}
