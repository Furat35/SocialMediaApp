using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace BuildingBlocks.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureConsul(this IServiceCollection services, IConfiguration configuration, Action<ConsulClientConfiguration> consulClientConfiguration = null)
        {
            var address = $"{configuration["ConsulConfig:Host"]}:{configuration["ConsulConfig:Port"]}";
            var consulClient = new ConsulClientConfiguration { Address = new Uri(address) };
            if (consulClientConfiguration is not null) consulClientConfiguration(consulClient);
            services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulClient));

            return services;
        }

        public static IApplicationBuilder AddConsulConfig(this IApplicationBuilder app, IHostApplicationLifetime lifetime, IConfiguration configuration, Action<AgentServiceRegistration> config = null)
        {
            var consulClient = app.ApplicationServices.GetRequiredService<IConsulClient>();

            var loggingFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();

            var logger = loggingFactory.CreateLogger<IApplicationBuilder>();
            //["ASPNETCORE_URLS"] bakılacak
            // Get server IP
            int port;
            if (configuration["ASPNETCORE_ENVIRONMENT"] == "Development")
            {
                port = new Uri(configuration["ASPNETCORE_URLS"]).Port;
            }
            else
            {
                port = int.Parse(configuration["ASPNETCORE_HTTP_PORTS"]!);
            }

            var ip = Dns.GetHostEntry(Dns.GetHostName()).AddressList
                    .FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork)?.ToString();
            var address = $"http://{ip}:{port}";
            var uri = new Uri(address);
            var currentAssembly = Assembly.GetEntryAssembly().GetName().Name.ToLower();

            Console.WriteLine($"Registered {currentAssembly} to address: {address}");
            var registration = new AgentServiceRegistration()
            {
                ID = $"{currentAssembly}-" + Guid.NewGuid(),
                Name = currentAssembly,
                Address = $"{uri.Host}",
                Port = uri.Port,
                Tags = [currentAssembly],
                Check = new AgentServiceCheck
                {
                    HTTP = $"{uri.Scheme}://{uri.Host}:{uri.Port}/health",
                    Interval = TimeSpan.FromSeconds(10),
                    Timeout = TimeSpan.FromSeconds(5),
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(20)
                }
            };

            if (config != null) config(registration);

            logger.LogInformation("Registering with Consul");
            consulClient.Agent.ServiceDeregister(registration.ID).Wait();
            consulClient.Agent.ServiceRegister(registration).Wait();

            lifetime.ApplicationStopping.Register(() =>
            {
                logger.LogInformation("Deregistering from Consul");
                consulClient.Agent.ServiceDeregister(registration.ID).Wait();
            });

            return app;
        }
    }
}
