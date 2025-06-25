using BuildingBlocks.Extensions;
using SocialMediaApp.Aggregator.Helpers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.ConfigureConsul(builder.Configuration);
builder.Services.AddHealthChecks();
builder.Services.ConfigureAuthentication(builder.Configuration);
builder.Services.AddAuthorization();
builder.Services.AddTransient<BearerTokenHandler>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient("default")
    .AddHttpMessageHandler<BearerTokenHandler>();

var app = builder.Build();

app.MapHealthChecks("/health");
var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
app.AddConsulConfig(lifetime, builder.Configuration);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
