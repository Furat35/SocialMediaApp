using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BuildingBlocks.Extensions
{
    public static class RegisterServices
    {
        public static IServiceCollection AddAllServices(
            this IServiceCollection services,
            List<Assembly> assemblies,
            ServiceLifetime lifetime = ServiceLifetime.Scoped,
            string postfix = "Service",
            bool searchAllAssemblies = true)
        {
            assemblies.ForEach(ass =>
                ass.GetTypes()
                    .Where(t => t.IsClass && !t.IsAbstract && !t.IsInterface && t.Name.EndsWith(postfix))
                    .ToList()
                    .ForEach(serviceType =>
                    {
                        var interfaceType = serviceType.GetInterfaces().FirstOrDefault(i => i.Name == $"I{serviceType.Name}");
                        if (interfaceType != null)
                        {
                            services.Add(new ServiceDescriptor(interfaceType, serviceType, lifetime));
                        }
                    }));
            return services;
        }
    }
}
