using Bonyan.Modularity;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BonServiceCollectionExtensions
    {
        public static WebApplication UseBonyan(
            this WebApplication services, Action<BonWebApplicationContext> configure)
        {
            var context = new BonWebApplicationContext(services);
            configure.Invoke(context);
            return services;
        }
    }
}