using BuildingBlocks.Models;
using Consul;
using Microsoft.AspNetCore.Mvc;
using SocialMediaApp.Aggregator.helpers;
using SocialMediaApp.Aggregator.Models.Stories;
using SocialMediaApp.Aggregator.Models.Users;
using System.Runtime.ConstrainedExecution;

namespace SocialMediaApp.Aggregator.Controllers
{
    public class StoriesController(IHttpClientFactory httpClientFactory, IConsulClient consulClient) : AggregatedBaseController
    {
        private readonly HttpClient? _httpClient = httpClientFactory.CreateClient("default");
        
        [HttpGet]
        public async Task<IActionResult> GetStories([FromQuery] PaginationRequestModel request)
        {
            var httpClient = httpClientFactory.CreateClient("default");

            var postServiceUrl = await consulClient.ResolveServiceUrl("posts.api");
            var storyResponse = await httpClient.GetFromJsonAsync<PaginationResponseModel<List<StoryListDto>>>($"{postServiceUrl}/api/stories?page={request.Page}&pageSize={request.PageSize}");

            if(!storyResponse.Data.Any())
                return Ok(storyResponse);

            var userIds = storyResponse.Data.Select(_ => _[0].UserId).ToList();
            var userRequestResult = await GetUsersWithGivenIdsAsync(userIds);
            foreach (var stories in storyResponse.Data)
            {
                var user = userRequestResult.First(_ => _.Id == stories[0].UserId);
                stories.ForEach(_ => _.User = user);
            }

            return Ok(storyResponse);
        }


        [NonAction]
        public async Task<List<UserListDto>> GetUsersWithGivenIdsAsync(List<int> userIds)
        {
            var identityServiceUrl = await consulClient.ResolveServiceUrl("identityserver.api");
            var request = await _httpClient.PostAsJsonAsync($"{identityServiceUrl}/api/users", userIds);

            var userResponse = await request.Content.ReadFromJsonAsync<ResponseDto<List<UserListDto>>>();

            return userResponse.Data;
        }
    }
}
