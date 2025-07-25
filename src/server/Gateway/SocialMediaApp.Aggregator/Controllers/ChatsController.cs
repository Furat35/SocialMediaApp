using BuildingBlocks.Models;
using Consul;
using Microsoft.AspNetCore.Mvc;
using SocialMediaApp.Aggregator.helpers;
using SocialMediaApp.Aggregator.Models.Users;

namespace SocialMediaApp.Aggregator.Controllers
{
    public class ChatsController(IHttpClientFactory httpClientFactory, IConsulClient consulClient) : AggregatedBaseController
    {
        private readonly HttpClient? _httpClient = httpClientFactory.CreateClient("default");

        [HttpGet]
        public async Task<IActionResult> GetLastUserChats([FromQuery] PaginationRequestModel request)
        {
            var httpClient = httpClientFactory.CreateClient("default");

            var chatServiceUrl = await consulClient.ResolveServiceUrl("chat.signalr");
            var messageResponse = await httpClient.GetFromJsonAsync<PaginationResponseModel<int>>($"{chatServiceUrl}/api/chats/lastchats?page={request.Page}&pageSize={request.PageSize}");

            var userRequestResult = await GetUsersWithGivenIdsAsync(messageResponse.Data);
            var newUserList = new List<UserListDto>();  
            foreach (var userId in messageResponse.Data)    
            {
                newUserList.Add(userRequestResult.First(_ => _.Id == userId));
            }
            var paginationModel = new PaginationResponseModel<UserListDto>(messageResponse.Page, messageResponse.PageSize, messageResponse.PageCount, messageResponse.TotalEntities, newUserList);

            return Ok(paginationModel);
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
