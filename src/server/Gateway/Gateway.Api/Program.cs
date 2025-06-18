using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(builder =>
{
    builder.AddPolicy("DefaultCorsPolicy", options =>
    {
        options.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddControllers();
builder.Services
    .AddOcelot()
    .AddConsul();

var app = builder.Build();

app.UseCors("DefaultCorsPolicy");
app.UseAuthorization();

app.MapControllers();
await app.UseOcelot();

app.Run();
