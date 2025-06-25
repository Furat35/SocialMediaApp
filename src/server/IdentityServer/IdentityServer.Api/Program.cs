using BuildingBlocks.Extensions;
using IdentityServer.Api.Data.Context;
using IdentityServer.Api.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services
    .AddIdentityServices(builder.Configuration)
    .ConfigureConsul(builder.Configuration);

builder.Services.AddHealthChecks();
builder.Services.ConfigureAuthentication(builder.Configuration);
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

try
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
    dbContext.Database.Migrate();
}
catch (Exception ex)
{
    Console.WriteLine($"Migration failed: {ex.Message}");
}

app.MapHealthChecks("/health");
var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
app.AddConsulConfig(lifetime, builder.Configuration);

app.MapControllers();

app.Run();
