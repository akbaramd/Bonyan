using Autofac;
using Bonyan.Modularity;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BonServiceCollectionExtensions
    {
        public static IServiceCollection AddBonyan(
            this IServiceCollection services, Action<BonConfigurationContext> configure)
        {
            var context = new BonConfigurationContext(services);
            configure.Invoke(context);
            return services;
        }
    }
}


namespace Microsoft.Extensions.DependencyInjection
{
    public static class BonServiceProviderExtensions
    {
        public static IServiceProvider InitializeBonyan(
            this IServiceProvider services, Action<BonInitializedContext> configure)
        {
            var context = new BonInitializedContext(services);
            configure.Invoke(context);
            return services;
        }
    }
}