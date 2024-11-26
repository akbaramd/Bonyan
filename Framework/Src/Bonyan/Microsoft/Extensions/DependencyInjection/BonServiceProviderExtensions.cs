using Bonyan.Modularity;

namespace Microsoft.Extensions.DependencyInjection;

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