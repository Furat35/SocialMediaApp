
using BuildingBlocks.Extensions;
using BuildingBlocks.Models;
using BuildingBlocks.Models.Enums;
using Consul;
using SocialMediaApp.Aggregator.Models.Posts;

namespace SocialMediaApp.Aggregator.Services
{
    public class PostService(IHttpClientFactory httpClientFactory, IConsulClient consulClient) : IPostService
    {
        private readonly HttpClient? _httpClient = httpClientFactory.CreateClient("default");
        private readonly string postsServiceUrl = consulClient.ResolveServiceUrl(ConsulServiceNames.Posts_Api).Result;
        public async Task<PaginationResponseModel<PostListDto>> GetPostsByUserId(PaginationRequestModel request, int userId)
        {
            var content = await _httpClient.GetAsync($"{postsServiceUrl}/api/posts?userId={userId}&page={request.Page}&pageSize={request.PageSize}");
            return await content.Content.ReadFromJsonAsync<PaginationResponseModel<PostListDto>>();
        }

        public async Task<PaginationResponseModel<PostListDto>> GetFollowerPosts(PaginationRequestModel request)
        {
            return await _httpClient.GetFromJsonAsync<PaginationResponseModel<PostListDto>>($"{postsServiceUrl}/api/posts/follower-posts?page={request.Page}&pageSize={request.PageSize}");
        }
    }

    public interface IPostService
    {
        Task<PaginationResponseModel<PostListDto>> GetPostsByUserId(PaginationRequestModel request, int userId);
        Task<PaginationResponseModel<PostListDto>> GetFollowerPosts(PaginationRequestModel request);
    }
}
