using BuildingBlocks.Extensions;
using IdentityServer.Api.Data.Context;
using IdentityServer.Api.Extensions;
using Microsoft.EntityFrameworkCore;
using BuildingBlocks.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddIdentityServices(builder.Configuration);

builder.Services.AddOpenApiDocument();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
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
}


app.HandleException();
app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");
var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
lifetime.ApplicationStarted.Register(() => app.AddConsulConfig(lifetime, builder.Configuration));

app.MapControllers();

app.Run();
