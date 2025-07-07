using Chat.SignalR.Data.Contexts;
using Chat.SignalR.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Chat.SignalR.Hubs
{
    [Authorize]
    public class ChatHub(ChatDbContext context) : Hub
    {
        public async Task SendMessage(int toUserId, string userMessage)
        {
            var userId = int.Parse(Context.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var message = new Message(userId, toUserId, userMessage);
            await context.AddAsync(message);
            await context.SaveChangesAsync();
            await Clients.User(toUserId.ToString()).SendAsync("ReceiveMessage", message);
        }
    }
}
