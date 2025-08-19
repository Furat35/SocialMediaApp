using BuildingBlocks.Extensions;
using BuildingBlocks.Models;
using BuildingBlocks.Models.Enums;
using Consul;
using SocialMediaApp.Aggregator.Models.Stories;

namespace SocialMediaApp.Aggregator.Services
{
    public class StoryService(IHttpClientFactory httpClientFactory, IConsulClient consulClient) : IStoryService
    {
        private readonly HttpClient? _httpClient = httpClientFactory.CreateClient("default");
        private readonly string storyServiceUrl = consulClient.ResolveServiceUrl(ConsulServiceNames.Stories_Api).Result;

        public async Task<PaginationResponseModel<List<StoryListDto>>> GetStories(PaginationRequestModel request)
        {
            var storyResponse = await _httpClient.GetFromJsonAsync<PaginationResponseModel<List<StoryListDto>>>($"{storyServiceUrl}/api/stories?page={request.Page}&pageSize={request.PageSize}");
            return storyResponse;
        }
    }

    public interface IStoryService
    {
        Task<PaginationResponseModel<List<StoryListDto>>> GetStories(PaginationRequestModel request);
    }
}
