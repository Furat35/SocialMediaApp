using BuildingBlocks.Extensions;
using BuildingBlocks.Models;
using BuildingBlocks.Models.Enums;
using Consul;
using SocialMediaApp.Aggregator.Models.Chats;

namespace SocialMediaApp.Aggregator.Services
{
    public class ChatService(IHttpClientFactory httpClientFactory, IConsulClient consulClient) : IChatService
    {
        private readonly HttpClient? _httpClient = httpClientFactory.CreateClient("default");
        private readonly string chatServiceUrl = consulClient.ResolveServiceUrl(ConsulServiceNames.ChatSignalR).Result;
        public async Task<PaginationResponseModel<MessageListDto>> GetLastChats(PaginationRequestModel request)
        {
            var chatResponse = await _httpClient.GetFromJsonAsync<PaginationResponseModel<MessageListDto>>($"{chatServiceUrl}/api/chats/lastchats?page={request.Page}&pageSize={request.PageSize}");
            return chatResponse;
        }

        public async Task<PaginationResponseModel<MessageListDto>> GetLastChatsByUserIds(PaginationRequestModel request, List<int> followerUserIds)
        {
            var messageRequest = await _httpClient.PostAsJsonAsync($"{chatServiceUrl}/api/chats/lastchats-byUserIds?page={request.Page}&pageSize={request.PageSize}", followerUserIds);
            return await messageRequest.Content.ReadFromJsonAsync<PaginationResponseModel<MessageListDto>>();
        }
    }

    public interface IChatService
    {
        Task<PaginationResponseModel<MessageListDto>> GetLastChats(PaginationRequestModel request);
        Task<PaginationResponseModel<MessageListDto>> GetLastChatsByUserIds(PaginationRequestModel request, List<int> followerUserIds);
    }
}
