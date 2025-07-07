using Chat.SignalR.Data.Contexts;
using Microsoft.AspNetCore.Mvc;
using BuildingBlocks.Extensions;
using BuildingBlocks.Models;
using Azure.Core;
using System.Net;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Chat.SignalR.Models;

namespace Chat.SignalR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatsController(IHttpContextAccessor httpContextAccessor, ChatDbContext context) : ControllerBase
    {
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetMessages([FromRoute] int userId, [FromQuery] PaginationRequestModel request)
        {
            var messages = context.Messages
                .Where(_ => (_.From == httpContextAccessor.GetUserId() && _.To == userId) || (_.To == httpContextAccessor.GetUserId() && _.From == userId));

            var totalMessages= await messages.CountAsync();
            var pageCount = totalMessages / request.PageSize + (totalMessages % request.PageSize > 0 ? 1 : 0);
            var response = await messages
                .OrderBy(_ => _.SentDate)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var paginationModel = new PaginationResponseModel<Message>(request.Page, request.PageSize, pageCount, totalMessages, response);

            return Ok(paginationModel);
        }
    }
}
