using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
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
            var host = configuration["ConsulConfig:Host"];
            var port = configuration["ConsulConfig:Port"];
            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(port))
            {
                Console.WriteLine("Consul config missing → skipping registration.");
                return services;
            }

            var address = $"{host}:{port}";


            services.AddSingleton<IConsulClient, ConsulClient>(p =>
                new ConsulClient(c =>
                {
                    c.Address = new Uri(address);
                    consulClientConfiguration?.Invoke(c);
                }));

            return services;
        }

        public static IApplicationBuilder AddConsulConfig(this IApplicationBuilder app, IHostApplicationLifetime lifetime, IConfiguration configuration, Action<AgentServiceRegistration> config = null)
        {
            if (string.IsNullOrWhiteSpace(configuration["ConsulConfig:Host"]) || string.IsNullOrWhiteSpace(configuration["ConsulConfig:Port"]))
            {
                Console.WriteLine("Consul config missing → skipping registration.");
                return app;
            }

            var consulClient = app.ApplicationServices.GetRequiredService<IConsulClient>();

            var loggingFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();

            var logger = loggingFactory.CreateLogger<IApplicationBuilder>();

            var server = app.ApplicationServices.GetRequiredService<IServer>();
            var addressesFeature = server.Features.Get<IServerAddressesFeature>();
            var address = addressesFeature?.Addresses.FirstOrDefault();
            if (address == null)
            {
                Console.WriteLine("No server address found for Consul registration.");
                return app;
            }

            var uri = new Uri(address);
            var host = uri.Host;
            if (host.Contains("::") || host == "0.0.0.0" || host == "localhost")
            {
                host = Dns.GetHostEntry(Dns.GetHostName())
                          .AddressList
                          .FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork)?
                          .ToString();
            }
            var currentAssembly = Assembly.GetEntryAssembly().GetName().Name.ToLower();

            Console.WriteLine($"Registered {currentAssembly} to address: {address}");
            var registration = new AgentServiceRegistration()
            {
                ID = $"{currentAssembly}-" + Guid.NewGuid(),
                Name = currentAssembly,
                Address = host,
                Port = uri.Port,
                Tags = [currentAssembly],
                Check = new AgentServiceCheck
                {
                    HTTP = $"{uri.Scheme}://{host}:{uri.Port}/health",
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
