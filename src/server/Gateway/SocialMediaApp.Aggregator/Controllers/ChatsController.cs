using BuildingBlocks.Extensions;
using BuildingBlocks.Models;
using Microsoft.AspNetCore.Mvc;
using SocialMediaApp.Aggregator.Models.Chats;
using SocialMediaApp.Aggregator.Services;

namespace SocialMediaApp.Aggregator.Controllers
{
    public class ChatsController(IHttpContextAccessor httpContext, IUserService userService,
        IChatService chatService, IFollowerService followerService)
        : AggregatedBaseController
    {
        [HttpGet]
        public async Task<IActionResult> GetLastUserChats([FromQuery] PaginationRequestModel request)
        {
            var chatResponse = await chatService.GetLastChats(request);

            var userIds = chatResponse.Data.Select(_ => _.From == httpContext.GetUserId() ? _.To : _.From).ToList();
            var userRequestResult = await userService.GetUsersWithGivenIdsAsync(userIds);
            foreach (var message in chatResponse.Data)
            {
                var id = message.From == httpContext.GetUserId() ? message.To : message.From;
                message.User = userRequestResult.Data.First(_ => _.Id == id);
            }

            return Ok(chatResponse);
        }

        [HttpPost("lastChats-byUserIds")]
        public async Task<IActionResult> GetLastUserChatsByUsername([FromQuery] PaginationRequestModel request,
            [FromQuery] string searchKey)
        {
            var userResponse = await userService.SearchUserAsync(searchKey);

            var followerResponse = await followerService.SearchFollower(userResponse.Data, request);

            var followerUserIds = followerResponse.Data.Select(_ => _.RespondingUserId == httpContext.GetUserId() ? _.RequestingUserId : _.RespondingUserId).ToList();
            var users = await userService.GetUsersWithGivenIdsAsync(followerUserIds);

            var messageResponse = await chatService.GetLastChatsByUserIds(request, followerUserIds);

            var newMessageList = new List<MessageListDto>();
            for (int i = 0; i < users.Data.Count; i++)
            {
                var message = messageResponse.Data.FirstOrDefault(_ => _.From == users.Data[i].Id || _.To == users.Data[i].Id);
                if (message is null)
                {
                    message = new MessageListDto();
                    message.IsRead = true;
                }

                message.User = users.Data[i];
                newMessageList.Add(message);
            }

            return Ok(new PaginationResponseModel<MessageListDto>(messageResponse.Page, messageResponse.PageSize, messageResponse.PageCount, messageResponse.TotalEntities, newMessageList));
        }

    }
}
