using BuildingBlocks.Extensions;
using SocialMediaApp.Aggregator.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAggregatorServices(builder.Configuration);

builder.Services.AddOpenApiDocument();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
}

app.MapHealthChecks("/health");
var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
lifetime.ApplicationStarted.Register(() => app.AddConsulConfig(lifetime, builder.Configuration));

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
