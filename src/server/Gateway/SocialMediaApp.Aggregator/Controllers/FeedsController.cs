using BuildingBlocks.Models;
using Microsoft.AspNetCore.Mvc;
using SocialMediaApp.Aggregator.Services;

namespace SocialMediaApp.Aggregator.Controllers
{
    public class FeedsController(IUserService userService, IPostService postService) : AggregatedBaseController
    {

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserPosts([FromRoute] int userId, [FromQuery] PaginationRequestModel request)
        {
            var postsResponse = await postService.GetPostsByUserId(request, userId);

            //if (!content.IsSuccessStatusCode)
            //    return new ObjectResult(postsResponse) { StatusCode = (int)content.StatusCode };

            if (postsResponse is null || !postsResponse.Data.Any())
                return Ok(postsResponse);

            var postUserIds = postsResponse.Data.First().UserId;
            var postUserResult = await userService.GetUsersWithGivenIdsAsync([postUserIds]);

            var commentUserIds = new List<int>();
            postsResponse.Data.ForEach(p => commentUserIds.AddRange(p.Comments.Select(c => c.UserId)));
            var commentUserResult = await userService.GetUsersWithGivenIdsAsync(commentUserIds);

            var likeUserIds = new List<int>();
            postsResponse.Data.ForEach(p => likeUserIds.AddRange(p.Likes.Select(c => c.UserId)));
            var likeUserResult = await userService.GetUsersWithGivenIdsAsync(likeUserIds);

            postsResponse.Data.ForEach(p =>
            {
                foreach (var comment in p.Comments)
                    comment.User = commentUserResult.Data.First(_ => _.Id == comment.UserId);

                foreach (var like in p.Likes)
                    like.User = likeUserResult.Data.First(_ => _.Id == like.UserId);

                p.User = postUserResult.Data.First();
            });


            return Ok(postsResponse);
        }

        [HttpGet]
        public async Task<IActionResult> GetFollowerPosts([FromQuery] PaginationRequestModel request)
        {
            //var identityServiceUrl = await consulClient.ResolveServiceUrl("identityserver.api");
            //var followersResponse = await httpClient.GetFromJsonAsync<PaginationResponseModel<FollowerListDto>>($"{identityServiceUrl}/api/followers/ids");


            var postsResponse = await postService.GetFollowerPosts(request);


            var postUserIds = postsResponse.Data.Select(p => p.UserId).Distinct().ToList();
            var postUserResult = await userService.GetUsersWithGivenIdsAsync(postUserIds);


            var commentUserIds = new List<int>();
            postsResponse.Data.ForEach(p => commentUserIds.AddRange(p.Comments.Select(c => c.UserId)));
            var commentUserResult = await userService.GetUsersWithGivenIdsAsync(commentUserIds);

            var likeUserIds = new List<int>();
            postsResponse.Data.ForEach(p => likeUserIds.AddRange(p.Likes.Select(c => c.UserId)));
            var likeUserResult = await userService.GetUsersWithGivenIdsAsync(likeUserIds);


            postsResponse.Data.ForEach(p =>
            {
                foreach (var comment in p.Comments)
                    comment.User = commentUserResult.Data.First(_ => _.Id == comment.UserId);

                foreach (var like in p.Likes)
                    like.User = likeUserResult.Data.First(_ => _.Id == like.UserId);

                p.User = postUserResult.Data.First(_ => _.Id == p.UserId);
            });


            return Ok(postsResponse);
        }
    }
}
