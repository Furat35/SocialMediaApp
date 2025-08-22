using BuildingBlocks.Extensions;
using BuildingBlocks.Interfaces.Services;
using IdentityServer.Api.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace IdentityServer.Api.Extensions
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAllServices([Assembly.GetExecutingAssembly(), Assembly.GetAssembly(typeof(IFileService))]);
            services.AddAllRepositories([Assembly.GetExecutingAssembly()]);
            services.AddAutoMapper(typeof(Program).Assembly);
            services.AddDbContext<IdentityDbContext>(opt => opt.UseSqlServer(configuration.GetConnectionString("IdentityDb")));
            services.AddControllers();
            services.AddHealthChecks();
            services.AddHttpContextAccessor();
            services.ConfigureAuthentication(configuration);
            services.ConfigureConsul(configuration);

            return services;
        }
    }
}
