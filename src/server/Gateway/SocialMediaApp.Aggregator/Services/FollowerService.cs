using BuildingBlocks.Extensions;
using BuildingBlocks.Models;
using BuildingBlocks.Models.Enums;
using Consul;
using SocialMediaApp.Aggregator.Models.Followers;

namespace SocialMediaApp.Aggregator.Services
{
    public class FollowerService(IHttpClientFactory httpClientFactory, IConsulClient consulClient) : IFollowerService
    {
        private readonly HttpClient? _httpClient = httpClientFactory.CreateClient("default");
        private readonly string identityServiceUrl = consulClient.ResolveServiceUrl(ConsulServiceNames.IdentityServer_Api).Result;
        public async Task<PaginationResponseModel<FollowerListDto>> GetFollowRequestsAsync(PaginationRequestModel request)
        {
            var followResponse = await _httpClient.GetFromJsonAsync<PaginationResponseModel<FollowerListDto>>($"{identityServiceUrl}/api/followers/follow-requests?page={request.Page}&pageSize={request.PageSize}");
            return followResponse;
        }

        public async Task<PaginationResponseModel<FollowerListDto>> GetFollowersByUserIdAsync(PaginationRequestModel request, int userId, int status)
        {
            return await _httpClient.GetFromJsonAsync<PaginationResponseModel<FollowerListDto>>($"{identityServiceUrl}/api/followers/byuser?status={status}&userId={userId}&page={request.Page}&pageSize={request.PageSize}");
        }

        public async Task<PaginationResponseModel<FollowerListDto>> SearchFollower(List<int> userIds, PaginationRequestModel request)
        {
            var followersRequest = await _httpClient.PostAsJsonAsync($"{identityServiceUrl}/api/followers/search?page={request.Page}&pageSize={request.PageSize}",
                            new { UserIds = userIds, request.Page, request.PageSize });
            return await followersRequest.Content.ReadFromJsonAsync<PaginationResponseModel<FollowerListDto>>();
        }
    }

    public interface IFollowerService
    {
        Task<PaginationResponseModel<FollowerListDto>> GetFollowRequestsAsync(PaginationRequestModel request);
        Task<PaginationResponseModel<FollowerListDto>> GetFollowersByUserIdAsync(PaginationRequestModel request, int userId, int status);
        Task<PaginationResponseModel<FollowerListDto>> SearchFollower(List<int> userIds, PaginationRequestModel request);
    }
}
