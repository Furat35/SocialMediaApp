using BuildingBlocks.Extensions;
using BuildingBlocks.Models;
using BuildingBlocks.Models.Enums;
using Consul;

namespace Posts.Api.ExternalServices
{
    public class FollowerService(IHttpClientFactory httpClientFactory, IConsulClient consulClient) : IFollowerService
    {
        private readonly HttpClient? _httpClient = httpClientFactory.CreateClient("default");
        public async Task<List<int>> GetFollowerIdsAsync()
        {
            var identityServiceUrl = await consulClient.ResolveServiceUrl(ConsulServiceNames.IdentityServer_Api);
            var content = await _httpClient.GetAsync($"{identityServiceUrl}/api/followers/ids");
            var followerResponse = await content.Content.ReadFromJsonAsync<ResponseDto<List<int>>>();

            return followerResponse.Data;
        }
    }

    public interface IFollowerService
    {
        Task<List<int>> GetFollowerIdsAsync();
    }
}
