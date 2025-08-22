using BuildingBlocks.Extensions;
using Chat.SignalR.Data.Contexts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Chat.SignalR.Extensions
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddChatServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.ConfigureConsul(configuration);
            services.AddHealthChecks();
            services.AddHttpContextAccessor();
            services.AddSignalR();
            services.AddCors(options =>
            {
                options.AddPolicy("default", policy =>
                {
                    policy
                        .SetIsOriginAllowed(_ => true) // frontend origin
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
            services.AddDbContext<ChatDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("ChatDb")));
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                        .AddJwtBearer(options =>
                        {
                            options.TokenValidationParameters = new TokenValidationParameters
                            {
                                ValidateIssuer = false,
                                ValidateAudience = false,
                                ValidateLifetime = true,
                                ValidateIssuerSigningKey = true,
                                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtConfig:SecretKey"])),
                                RequireExpirationTime = true
                            };
                            options.Events = new JwtBearerEvents
                            {
                                OnMessageReceived = context =>
                                {
                                    var accessToken = context.Request.Query["access_token"];
                                    var path = context.HttpContext.Request.Path;
                                    if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chatHub"))
                                    {
                                        context.Token = accessToken;
                                    }

                                    return Task.CompletedTask;
                                }
                            };
                        });


            services.AddHttpContextAccessor();
            services.AddAuthorization();

            return services;
        }
    }
}
