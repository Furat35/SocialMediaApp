using BuildingBlocks.Controllers;
using BuildingBlocks.Models;
using Consul;
using Microsoft.AspNetCore.Mvc;
using SocialMediaApp.Aggregator.helpers;
using SocialMediaApp.Aggregator.Models.Posts;
using SocialMediaApp.Aggregator.Models.Users;

namespace SocialMediaApp.Aggregator.Controllers
{
    public class FeedsController(IHttpClientFactory httpClientFactory, IConsulClient consulClient) : BaseController
    {
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserPosts(int userId)
        {
            var httpClient = httpClientFactory.CreateClient("default");

            var postsServiceUrl = await consulClient.ResolveServiceUrl("posts.api");
            var postsResponse = httpClient.GetFromJsonAsync<ResponseDto<PaginationResponseModel<PostListDto>>>($"{postsServiceUrl}/api/posts?userId={userId}");

            var identityServiceUrl = await consulClient.ResolveServiceUrl("identityserver.api");
            var userResponse = httpClient.GetFromJsonAsync<ResponseDto<UserListDto>>($"{identityServiceUrl}/api/users/{userId}");

            await Task.WhenAll(postsResponse, userResponse);

            var posts = postsResponse.Result.Data.Data;
            var user = userResponse.Result.Data;

            posts?.ForEach(_ => _.User = user);

            return CreateActionResult(postsResponse.Result);
        }

        [HttpGet]
        public async Task<IActionResult> GetFollowerPosts([FromQuery] PaginationRequestModel request)
        {
            var httpClient = httpClientFactory.CreateClient("default");

            var postsServiceUrl = await consulClient.ResolveServiceUrl("posts.api");
            var postsResponse = await httpClient.GetFromJsonAsync<ResponseDto<PaginationResponseModel<PostListDto>>>($"{postsServiceUrl}/api/posts/follower-posts?page={request.Page}&pageSize={request.PageSize}");
            var userIds = postsResponse.Data.Data.Select(p => p.UserId).ToList();   
            
            var identityServiceUrl = await consulClient.ResolveServiceUrl("identityserver.api");
            var userResponse = await httpClient.PostAsJsonAsync($"{identityServiceUrl}/api/users", userIds);
            var userResult = await userResponse.Content.ReadFromJsonAsync<ResponseDto<List<UserListDto>>>();

            postsResponse.Data.Data.ForEach(p =>
            {
                p.User = userResult.Data.FirstOrDefault(_ => _.Id ==p.UserId);
            });


            return CreateActionResult(postsResponse);
        }
    }
}
