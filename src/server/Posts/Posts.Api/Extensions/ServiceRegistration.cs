using BuildingBlocks.Extensions;
using Microsoft.EntityFrameworkCore;
using Posts.Api.Core.Application.Repositories;
using Posts.Api.Core.Application.Services;
using Posts.Api.Infrastructure.Repositories;
using Posts.Api.Infrastructure.Services;
using System.Reflection;

namespace Posts.Api.Extensions
{
    public static class ServiceRegistration
    {
        public static void AddPostsApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddDbContext<PostDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("PostsDb")));
            services.ConfigureConsul(configuration);
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddAutoMapper(typeof(Program).Assembly);
            services.AddHealthChecks();
            services.AddHttpContextAccessor();
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IFollowerRepository, FollowerRepository>();
            services.AddScoped<IImageService, ImageService>();
            services.ConfigureAuthentication(configuration);

        }
    }
}
