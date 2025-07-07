using Chat.SignalR.Models;
using Microsoft.EntityFrameworkCore;

namespace Chat.SignalR.Data.Contexts
{
    public class ChatDbContext(DbContextOptions<ChatDbContext> options) : DbContext(options)
    {
        public DbSet<Message> Messages { get; set; }
    }
}
