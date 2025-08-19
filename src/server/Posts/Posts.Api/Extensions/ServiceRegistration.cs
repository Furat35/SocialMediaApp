using BuildingBlocks.Extensions;
using BuildingBlocks.Helpers;
using BuildingBlocks.Interfaces.Services;
using BuildingBlocks.Services;
using Microsoft.EntityFrameworkCore;
using Posts.Api.Core.Application.Repositories;
using Posts.Api.ExternalServices;
using Posts.Api.Infrastructure.Repositories;
using System.Reflection;

namespace Posts.Api.Extensions
{
    public static class ServiceRegistration
    {
        public static void AddPostsApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddTransient<BearerTokenHandler>();
            services.AddHttpClient("default")
                .AddHttpMessageHandler<BearerTokenHandler>();
            services.AddDbContext<PostDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("PostsDb")));
            services.ConfigureConsul(configuration);
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddAutoMapper(typeof(Program).Assembly);
            services.AddHealthChecks();
            services.AddHttpContextAccessor();
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IFollowerService, FollowerService>();
            services.AddScoped<IFileService, FileService>();
            services.ConfigureAuthentication(configuration);

        }
    }
}
