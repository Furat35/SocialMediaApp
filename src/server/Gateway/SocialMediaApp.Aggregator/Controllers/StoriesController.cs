using BuildingBlocks.Models;
using Microsoft.AspNetCore.Mvc;
using SocialMediaApp.Aggregator.Models.Stories;
using SocialMediaApp.Aggregator.Services;

namespace SocialMediaApp.Aggregator.Controllers
{
    public class StoriesController(IUserService userService, IStoryService storyService)
        : AggregatedBaseController
    {
        [HttpGet]
        public async Task<ActionResult<PaginationResponseModel<StoryListDto>>> GetStories([FromQuery] PaginationRequestModel request)
        {
            var storyResponse = await storyService.GetStories(request);
            if (!storyResponse.Data.Any())
                return Ok(storyResponse);

            var userIds = storyResponse.Data.Select(_ => _[0].UserId).ToList();
            var userRequestResult = await userService.GetUsersWithGivenIdsAsync(userIds);
            foreach (var stories in storyResponse.Data)
            {
                var user = userRequestResult.Data.First(_ => _.Id == stories[0].UserId);
                stories.ForEach(_ => _.User = user);
            }

            return Ok(storyResponse);
        }
    }
}
