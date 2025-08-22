using BuildingBlocks.Extensions;
using BuildingBlocks.Helpers;
using BuildingBlocks.Interfaces.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Stories.Api.Behaviours;
using Stories.Api.Infrastructure.Repositories;
using System.Reflection;

namespace Stories.Api.Extensions
{
    public static class ServiceRegistration
    {
        public static void AddStoriesApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(FollowerAuthorizationBehavior<,>));
            services.AddTransient<BearerTokenHandler>();
            services.AddHttpClient("default")
                .AddHttpMessageHandler<BearerTokenHandler>();
            services.AddDbContext<StoryDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("StoriesDb")));
            services.ConfigureConsul(configuration);
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddAutoMapper(typeof(Program).Assembly);
            services.AddHealthChecks();
            services.AddHttpContextAccessor();
            services.AddAllServices([Assembly.GetExecutingAssembly(), Assembly.GetAssembly(typeof(IFileService))]);
            services.AddAllRepositories([Assembly.GetExecutingAssembly()]);
            services.ConfigureAuthentication(configuration);

        }
    }
}
