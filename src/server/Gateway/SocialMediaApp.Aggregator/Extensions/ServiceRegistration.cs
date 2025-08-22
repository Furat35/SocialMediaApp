using BuildingBlocks.Extensions;
using BuildingBlocks.Helpers;
using BuildingBlocks.Interfaces.Services;
using System.Reflection;

namespace SocialMediaApp.Aggregator.Extensions
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddAggregatorServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.ConfigureConsul(configuration);
            services.AddHealthChecks();
            services.ConfigureAuthentication(configuration);
            services.AddAuthorization();
            services.AddTransient<BearerTokenHandler>();
            services.AddAllServices([Assembly.GetExecutingAssembly(), Assembly.GetAssembly(typeof(IFileService))]);
            services.AddAllRepositories([Assembly.GetExecutingAssembly()]);
            services.AddHttpContextAccessor();
            services.AddHttpClient("default")
                .AddHttpMessageHandler<BearerTokenHandler>();

            return services;
        }
    }
}
