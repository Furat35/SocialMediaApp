using BuildingBlocks.Extensions;
using BuildingBlocks.Models;
using Chat.SignalR.Data.Contexts;
using Chat.SignalR.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Chat.SignalR.Controllers
{
    // feature: yeni mesaj geldiğinde mesajın okunmadığı işaretlenmesi 
    [Route("api/[controller]")]
    [ApiController]
    public class ChatsController(IHttpContextAccessor httpContextAccessor, ChatDbContext context) : ControllerBase
    {
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetMessages([FromRoute] int userId, [FromQuery] PaginationRequestModel request)
        {
            var messages = context.Messages
                .Where(_ => (_.From == httpContextAccessor.GetUserId() && _.To == userId) || (_.From == userId && _.To == httpContextAccessor.GetUserId()));

            var totalMessages = await messages.CountAsync();
            var pageCount = (int)Math.Ceiling((double)totalMessages / request.PageSize);
            var response = await messages
                .OrderByDescending(_ => _.SentDate)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var paginationModel = new PaginationResponseModel<Message>(request.Page, request.PageSize, pageCount, totalMessages, response);

            return Ok(paginationModel);
        }

        [HttpPost("lastChats-byUserIds")]
        public async Task<IActionResult> GetLastChatsByUserIds([FromQuery] PaginationRequestModel request, [FromBody] List<int> userIds)
        {
            var userId = httpContextAccessor.GetUserId();
            //var messages = await context.Messages
            //     .FromSqlRaw(@"
            //        WITH RankedMessages AS (
            //            SELECT *,
            //                ROW_NUMBER() OVER (
            //                    PARTITION BY 
            //                        CASE WHEN [From] < [To] THEN [From] ELSE [To] END,
            //                        CASE WHEN [From] < [To] THEN [To] ELSE [From] END
            //                    ORDER BY SentDate DESC
            //                ) AS rn
            //            FROM Messages
            //            WHERE [From] = {0} OR [To] = {0}
            //        )
            //        SELECT * FROM RankedMessages WHERE rn = 1 ", userId)
            //     .ToListAsync();
            var messages = await context.Messages
                  .Where(m =>
                      (m.From == userId && userIds.Contains(m.To)) ||
                      (m.To == userId && userIds.Contains(m.From)))
                  .GroupBy(m => new
                  {
                      A = m.From < m.To ? m.From : m.To,
                      B = m.From < m.To ? m.To : m.From
                  })
                  .Select(g => g.OrderByDescending(m => m.SentDate).First())
                  .ToListAsync();

            var totalMessages = messages.Count();
            var pageCount = totalMessages / request.PageSize + (totalMessages % request.PageSize > 0 ? 1 : 0);
            var response = messages
                .OrderByDescending(_ => _.SentDate)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();
            var paginationModel = new PaginationResponseModel<Message>(request.Page, request.PageSize, pageCount, totalMessages, response);

            return Ok(paginationModel);
        }

        [HttpGet("lastChats")]
        public async Task<IActionResult> LastChats([FromQuery] PaginationRequestModel request)
        {
            var userId = httpContextAccessor.GetUserId();
            var messages = await context.Messages
                 .FromSqlRaw(@"
                    WITH RankedMessages AS (
                        SELECT *,
                            ROW_NUMBER() OVER (
                                PARTITION BY 
                                    CASE WHEN [From] < [To] THEN [From] ELSE [To] END,
                                    CASE WHEN [From] < [To] THEN [To] ELSE [From] END
                                ORDER BY SentDate DESC
                            ) AS rn
                        FROM Messages
                        WHERE [From] = {0} OR [To] = {0}
                    )
                    SELECT * FROM RankedMessages WHERE rn = 1 ", userId)
                 .ToListAsync();

            var totalMessages = messages.Count();
            var pageCount = totalMessages / request.PageSize + (totalMessages % request.PageSize > 0 ? 1 : 0);
            var response = messages
                .OrderByDescending(_ => _.SentDate)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();
            var paginationModel = new PaginationResponseModel<Message>(request.Page, request.PageSize, pageCount, totalMessages, response);

            return Ok(paginationModel);
        }

        [HttpPost("set-read")]
        public async Task SetMessagesAsRead([FromQuery] int userId)
        {
            await context.Messages
                .Where(_ => _.To == httpContextAccessor.GetUserId() && _.From == userId && !_.IsRead)
                .ExecuteUpdateAsync(_ => _.SetProperty(m => m.IsRead, true));
        }
    }
}
