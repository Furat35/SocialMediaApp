using BuildingBlocks.Extensions;
using BuildingBlocks.Middlewares;
using Microsoft.EntityFrameworkCore;
using Stories.Api.Extensions;
using Stories.Api.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddStoriesApiServices(builder.Configuration);

builder.Services.AddOpenApiDocument();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
}
try
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<StoryDbContext>();
    dbContext.Database.Migrate();
}
catch (Exception ex)
{
    Console.WriteLine($"Migration failed: {ex.Message}");
}

app.HandleException();
app.MapHealthChecks("/health");
var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
lifetime.ApplicationStarted.Register(() => app.AddConsulConfig(lifetime, builder.Configuration));


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
