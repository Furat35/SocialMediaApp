using BuildingBlocks.Extensions;
using BuildingBlocks.Models;
using Consul;
using Microsoft.AspNetCore.Mvc;
using SocialMediaApp.Aggregator.helpers;
using SocialMediaApp.Aggregator.Models.Chats;
using SocialMediaApp.Aggregator.Models.Followers;
using SocialMediaApp.Aggregator.Models.Users;

namespace SocialMediaApp.Aggregator.Controllers
{
    public class ChatsController(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContext, IConsulClient consulClient) : AggregatedBaseController
    {
        private readonly HttpClient? _httpClient = httpClientFactory.CreateClient("default");

        [HttpGet]
        public async Task<IActionResult> GetLastUserChats([FromQuery] PaginationRequestModel request)
        {
            var httpClient = httpClientFactory.CreateClient("default");

            var chatServiceUrl = await consulClient.ResolveServiceUrl("chat.signalr");
            var messageResponse = await httpClient.GetFromJsonAsync<PaginationResponseModel<MessageListDto>>($"{chatServiceUrl}/api/chats/lastchats?page={request.Page}&pageSize={request.PageSize}");

            var userIds = messageResponse.Data.Select(_ => _.From == httpContext.GetUserId() ? _.To : _.From).ToList();
            var userRequestResult = await GetUsersWithGivenIdsAsync(userIds);
            foreach (var message in messageResponse.Data)
            {
                var id = message.From == httpContext.GetUserId() ? message.To : message.From;
                message.User = userRequestResult.First(_ => _.Id == id);
            }

            return Ok(messageResponse);
        }

        [HttpPost("lastChats-byUserIds")]
        public async Task<IActionResult> GetLastUserChatsByUsername([FromQuery] PaginationRequestModel request, [FromQuery] string searchKey)
        {
            var httpClient = httpClientFactory.CreateClient("default");

            var identityServiceUrl = await consulClient.ResolveServiceUrl("identityserver.api");
            var userResponse = await _httpClient.GetFromJsonAsync<ResponseDto<List<int>>>($"{identityServiceUrl}/api/users/search?searchKey={searchKey}");

            var postServiceUrl = await consulClient.ResolveServiceUrl("posts.api");
            var postResponse = await _httpClient.PostAsJsonAsync($"{postServiceUrl}/api/followers/search?page={request.Page}&pageSize={request.PageSize}",
                new { UserIds = userResponse.Data, request.Page, request.PageSize });

            var followerResponse = await postResponse.Content.ReadFromJsonAsync<PaginationResponseModel<FollowerListDto>>();
            var followerUserIds = followerResponse.Data.Select(_ => _.RespondingUserId == httpContext.GetUserId() ? _.RequestingUserId : _.RespondingUserId).ToList();
            var users = await GetUsersWithGivenIdsAsync(followerUserIds);

            var chatServiceUrl = await consulClient.ResolveServiceUrl("chat.signalr");
            var messageRequest = await httpClient.PostAsJsonAsync($"{chatServiceUrl}/api/chats/lastchats-byUserIds?page={request.Page}&pageSize={request.PageSize}", followerUserIds);
            var messageResponse = await messageRequest.Content.ReadFromJsonAsync<PaginationResponseModel<MessageListDto>>();

            var newMessageList = new List<MessageListDto>();
            for (int i = 0; i < users.Count; i++)
            {
                var message = messageResponse.Data.FirstOrDefault(_ => _.From == users[i].Id || _.To == users[i].Id);
                if (message is null)
                {
                    message = new MessageListDto();
                    message.IsRead = true;
                }

                message.User = users[i];
                newMessageList.Add(message);
            }

            return Ok(new PaginationResponseModel<MessageListDto>(messageResponse.Page, messageResponse.PageSize, messageResponse.PageCount, messageResponse.TotalEntities, newMessageList));
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
