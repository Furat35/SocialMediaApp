using Consul;

namespace SocialMediaApp.Aggregator.helpers
{
    public static class ConsulServiceResolver
    {
        public static async Task<string> ResolveServiceUrl(this IConsulClient consulClient, string serviceName)
        {
            var services = await consulClient.Health.Service(serviceName, tag: null, passingOnly: true);
            var service = services.Response[new Random().Next(0, services.Response.Length)];

            if (service == null)
                throw new Exception($"{serviceName} not found in Consul");

            return $"http://{service.Service.Address}:{service.Service.Port}";
        }
    }
}
