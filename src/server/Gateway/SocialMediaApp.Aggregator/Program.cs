using BuildingBlocks.Extensions;
using SocialMediaApp.Aggregator.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAggregatorServices(builder.Configuration);

var app = builder.Build();

app.MapHealthChecks("/health");
var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
app.AddConsulConfig(lifetime, builder.Configuration);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
