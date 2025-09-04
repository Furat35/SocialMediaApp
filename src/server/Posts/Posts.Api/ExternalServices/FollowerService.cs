using BuildingBlocks.Extensions;
using BuildingBlocks.Models;
using BuildingBlocks.Models.Enums;
using Consul;

namespace Posts.Api.ExternalServices
{
    public class FollowerService : IFollowerService
    {
        private readonly HttpClient? _httpClient;

        public FollowerService(IHttpClientFactory httpClientFactory, IConsulClient consulClient)
        {
            _httpClient = httpClientFactory.CreateClient("default");
            var _identityServiceUrl = consulClient.ResolveServiceUrl(ConsulServiceNames.IdentityServer_Api).Result;
            _httpClient.BaseAddress = new Uri(_identityServiceUrl);
        }

        public async Task<List<int>> GetFollowerIdsAsync()
        {
            var content = await _httpClient.GetAsync($"api/followers/ids");
            return (await content.Content.ReadFromJsonAsync<ResponseDto<List<int>>>()).Data;
        }

        public async Task<bool> HasAccessToUser(int userId)
        {
            var content = await _httpClient.GetAsync($"api/followers/hasAccessTo?userId={userId}");
            return await content.Content.ReadFromJsonAsync<bool>();
        }
    }

    public interface IFollowerService
    {
        Task<List<int>> GetFollowerIdsAsync();
        Task<bool> HasAccessToUser(int userId);
    }
}
