using BuildingBlocks.Extensions;
using BuildingBlocks.Helpers;
using BuildingBlocks.Middlewares;
using Microsoft.EntityFrameworkCore;
using Posts.Api.Extensions;
using Posts.Api.Infrastructure.Repositories;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPostsApiServices(builder.Configuration);


//builder.Services.AddHttpClientRegistrations(builder.Configuration.Get<cons>);

//builder.Services.AddHttpClient("identityserver.api", async client =>
//{
//    var identityService = await consulClient.ResolveServiceUrl("identityserver.api");
//    client.BaseAddress = new Uri(identityService);
//});
builder.Services.AddOpenApiDocument();


var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
    try
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<PostDbContext>();
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Migration failed: {ex.Message}");
    }
}

app.HandleException();
app.MapHealthChecks("/health");
var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
lifetime.ApplicationStarted.Register(() => app.AddConsulConfig(lifetime, builder.Configuration));

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
