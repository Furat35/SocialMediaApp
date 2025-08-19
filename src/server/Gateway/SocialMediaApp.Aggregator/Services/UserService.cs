using BuildingBlocks.Extensions;
using BuildingBlocks.Models;
using BuildingBlocks.Models.Enums;
using Consul;
using SocialMediaApp.Aggregator.Models.Users;

namespace SocialMediaApp.Aggregator.Services
{
    public class UserService(IHttpClientFactory httpClientFactory, IConsulClient consulClient) : IUserService
    {
        private readonly HttpClient? _httpClient = httpClientFactory.CreateClient("default");
        private readonly string identityServiceUrl = consulClient.ResolveServiceUrl(ConsulServiceNames.IdentityServer_Api).Result;
        public async Task<ResponseDto<List<UserListDto>>> GetUsersWithGivenIdsAsync(List<int> userIds)
        {
            var request = await _httpClient.PostAsJsonAsync($"{identityServiceUrl}/api/users", userIds);
            var userResponse = await request.Content.ReadFromJsonAsync<ResponseDto<List<UserListDto>>>();

            return userResponse;
        }

        public async Task<ResponseDto<List<int>>> SearchUserAsync(string searchKey)
        {
            var userResponse = await _httpClient.GetFromJsonAsync<ResponseDto<List<int>>>($"{identityServiceUrl}/api/users/search?searchKey={searchKey}");
            return userResponse;
        }
    }

    public interface IUserService
    {
        Task<ResponseDto<List<UserListDto>>> GetUsersWithGivenIdsAsync(List<int> userIds);
        Task<ResponseDto<List<int>>> SearchUserAsync(string searchKey);
    }
}
