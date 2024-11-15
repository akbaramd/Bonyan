using Bonyan.Messaging;

namespace Microsoft.Extensions.DependencyInjection;

public static class BonMessagingServiceCollectionExtensions
{
    public static IServiceCollection AddBonMessaging(
        this IServiceCollection services,
        string serviceName ,
        Action<BonMessagingOptions> configureOptions)
    {
        var options = new BonMessagingOptions
        {
            ServiceName = serviceName,
        };

        configureOptions(options);

        // Register BonMessagingOptions in the service collection
        services.AddSingleton(options);

        // Register the dispatcher
        options.RegisterDispatcher(services);

        // Register consumers
        options.RegisterConsumers(services);

        return services;
    }
}