using Consul;

namespace BuildingBlocks.Extensions
{
    public static class ConsulServiceResolver
    {
        public static async Task<string> ResolveServiceUrl(this IConsulClient consulClient, string serviceName)
        {
            var services = await consulClient.Health.Service(serviceName, tag: null, passingOnly: true);
            if (services.Response == null || services.Response.Length == 0)
                throw new Exception($"{serviceName} not found in Consul");

            var service = services.Response[new Random().Next(0, services.Response.Length)];

            return $"http://{service.Service.Address}:{service.Service.Port}";
        }
    }
}
