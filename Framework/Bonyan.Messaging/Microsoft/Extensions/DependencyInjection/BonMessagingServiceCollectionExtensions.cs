using Bonyan.Messaging;

namespace Microsoft.Extensions.DependencyInjection;

public static class BonMessagingServiceCollectionExtensions
{
    public static IServiceCollection AddBonMessaging(
        this IServiceCollection services,
        string serviceName,
        Action<BonMessagingConfiguration> configureOptions)
    {
        var options = new BonMessagingConfiguration(services, serviceName);

        options.UseDispatcher<InMemoryBonMessageDispatcher>();
        
        configureOptions(options);

        // Register BonMessagingConfiguration in the service collection
        services.AddSingleton(options);

        // Register the dispatcher
        options.RegisterDispatcher();

        // Register consumers
        options.RegisterConsumers();

        return services;
    }
}