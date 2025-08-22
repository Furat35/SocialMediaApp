using BuildingBlocks.Extensions;
using BuildingBlocks.Helpers;
using BuildingBlocks.Interfaces.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Posts.Api.Behaviours;
using Posts.Api.Infrastructure.Repositories;
using System.Reflection;

namespace Posts.Api.Extensions
{
    public static class ServiceRegistration
    {
        public static void AddPostsApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(FollowerAuthorizationBehavior<,>));
            services.AddTransient<BearerTokenHandler>();
            services.AddHttpClient("default")
                .AddHttpMessageHandler<BearerTokenHandler>();
            services.AddDbContext<PostDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("PostsDb")));
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
