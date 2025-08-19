using BuildingBlocks.Interfaces.Services;
using BuildingBlocks.Services;
using IdentityServer.Api.Business;
using IdentityServer.Api.Business.Interfaces;
using IdentityServer.Api.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Api.Extensions
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IFollowerService, FollowerService>();
            services.AddAutoMapper(typeof(Program).Assembly);
            services.AddDbContext<IdentityDbContext>(opt =>
            {
                opt.UseSqlServer(configuration.GetConnectionString("IdentityDb"));
            });
            services.AddControllers();

            return services;
        }
    }
}
