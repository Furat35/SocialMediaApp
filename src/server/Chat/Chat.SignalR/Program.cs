using BuildingBlocks.Extensions;
using Chat.SignalR.Data.Contexts;
using Chat.SignalR.Extensions;
using Chat.SignalR.Hubs;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddChatServices(builder.Configuration);

var app = builder.Build();

app.MapHealthChecks("/health");
var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
app.AddConsulConfig(lifetime, builder.Configuration);

try
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ChatDbContext>();
    dbContext.Database.Migrate();
}
catch (Exception ex)
{
    Console.WriteLine($"Migration failed: {ex.Message}");
}

app.UseCors("default");


app.UseAuthentication();
app.UseAuthorization();

app.MapHub<ChatHub>("/chatHub");
app.MapControllers();

app.Run();

