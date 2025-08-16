using BuildingBlocks.Models;
using Consul;
using Microsoft.AspNetCore.Mvc;
using SocialMediaApp.Aggregator.helpers;
using SocialMediaApp.Aggregator.Models.Posts;
using SocialMediaApp.Aggregator.Models.Users;

namespace SocialMediaApp.Aggregator.Controllers
{
    public class FeedsController(IHttpClientFactory httpClientFactory, IConsulClient consulClient) : AggregatedBaseController
    {
        private readonly HttpClient? _httpClient = httpClientFactory.CreateClient("default");

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserPosts([FromRoute] int userId, [FromQuery] PaginationRequestModel request)
        {
            var postsServiceUrl = await consulClient.ResolveServiceUrl("posts.api");
            var content = await _httpClient.GetAsync($"{postsServiceUrl}/api/posts?userId={userId}&page={request.Page}&pageSize={request.PageSize}");
            var postsResponse = await content.Content.ReadFromJsonAsync<PaginationResponseModel<PostListDto>>();

            if (!content.IsSuccessStatusCode)
                return new ObjectResult(postsResponse) { StatusCode = (int)content.StatusCode };

            if (postsResponse.Data is null || !postsResponse.Data.Any())
                return Ok(postsResponse);

            var postUserIds = postsResponse.Data.First().UserId;
            var postUserResult = await GetUsersWithGivenIdsAsync([postUserIds]);

            var commentUserIds = new List<int>();
            postsResponse.Data.ForEach(p => commentUserIds.AddRange(p.Comments.Select(c => c.UserId)));
            var commentUserResult = await GetUsersWithGivenIdsAsync(commentUserIds);

            var likeUserIds = new List<int>();
            postsResponse.Data.ForEach(p => likeUserIds.AddRange(p.Likes.Select(c => c.UserId)));
            var likeUserResult = await GetUsersWithGivenIdsAsync(likeUserIds);

            postsResponse.Data.ForEach(p =>
            {
                foreach (var comment in p.Comments)
                    comment.User = commentUserResult.First(_ => _.Id == comment.UserId);

                foreach (var like in p.Likes)
                    like.User = likeUserResult.First(_ => _.Id == like.UserId);

                p.User = postUserResult.First();
            });


            return Ok(postsResponse);
        }

        [HttpGet]
        public async Task<IActionResult> GetFollowerPosts([FromQuery] PaginationRequestModel request)
        {
            var httpClient = httpClientFactory.CreateClient("default");

            var postsServiceUrl = await consulClient.ResolveServiceUrl("posts.api");
            var postsResponse = await httpClient.GetFromJsonAsync<PaginationResponseModel<PostListDto>>($"{postsServiceUrl}/api/posts/follower-posts?page={request.Page}&pageSize={request.PageSize}");


            var postUserIds = postsResponse.Data.Select(p => p.UserId).Distinct().ToList();
            var postUserResult = await GetUsersWithGivenIdsAsync(postUserIds);


            var commentUserIds = new List<int>();
            postsResponse.Data.ForEach(p => commentUserIds.AddRange(p.Comments.Select(c => c.UserId)));
            var commentUserResult = await GetUsersWithGivenIdsAsync(commentUserIds);

            var likeUserIds = new List<int>();
            postsResponse.Data.ForEach(p => likeUserIds.AddRange(p.Likes.Select(c => c.UserId)));
            var likeUserResult = await GetUsersWithGivenIdsAsync(likeUserIds);


            postsResponse.Data.ForEach(p =>
            {
                foreach (var comment in p.Comments)
                    comment.User = commentUserResult.First(_ => _.Id == comment.UserId);

                foreach (var like in p.Likes)
                    like.User = likeUserResult.First(_ => _.Id == like.UserId);

                p.User = postUserResult.First(_ => _.Id == p.UserId);
            });


            return Ok(postsResponse);
        }

        [NonAction]
        private async Task<List<UserListDto>> GetUsersWithGivenIdsAsync(List<int> userIds)
        {
            var identityServiceUrl = await consulClient.ResolveServiceUrl("identityserver.api");
            var request = await _httpClient.PostAsJsonAsync($"{identityServiceUrl}/api/users", userIds);

            var userResponse = await request.Content.ReadFromJsonAsync<ResponseDto<List<UserListDto>>>();

            return userResponse.Data;
        }
    }
}
