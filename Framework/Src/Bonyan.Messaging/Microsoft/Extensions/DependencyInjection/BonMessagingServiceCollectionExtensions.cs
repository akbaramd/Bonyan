using Bonyan.Messaging;
using Bonyan.Messaging.Abstractions;
using Bonyan.Modularity;

namespace Microsoft.Extensions.DependencyInjection;

public static class BonMessagingServiceCollectionExtensions
{
    public static BonConfigurationContext AddMessaging(
        this BonConfigurationContext context,
        string serviceName,
        Action<BonMessagingConfiguration> configureOptions)
    {

        var options = new BonMessagingConfiguration(context, serviceName);

        configureOptions(options);

        return context;
    }
}