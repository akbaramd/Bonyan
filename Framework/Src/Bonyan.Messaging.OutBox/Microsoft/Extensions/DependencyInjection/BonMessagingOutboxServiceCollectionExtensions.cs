using Bonyan.Messaging;
using Bonyan.Messaging.Abstractions;
using Bonyan.Messaging.OutBox;
using Bonyan.Messaging.OutBox.HostedServices;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for configuring the outbox pattern in the messaging system.
    /// </summary>
    public static class BonMessagingOutboxServiceCollectionExtensions
    {
        /// <summary>
        /// Adds outbox pattern support to the messaging configuration.
        /// This enables reliable messaging by storing messages in an outbox before processing.
        /// </summary>
        /// <param name="configuration">The messaging configuration.</param>
        /// <param name="action">Optional action to configure outbox settings.</param>
        /// <returns>The updated messaging configuration.</returns>
        public static BonMessagingConfiguration AddOutbox(
            this BonMessagingConfiguration configuration, 
            Action<BonMessagingOutBoxConfiguration>? action = null)
        {
            // Register outbox services
            configuration.Context.Services.AddSingleton<IOutboxStore, InMemoryOutboxStore>();
            configuration.Context.Services.Replace(ServiceDescriptor.Singleton<IBonMessageBus, BonOutBoxMessageBox>());
            configuration.Context.Services.AddHostedService<OutboxProcessorHostedService>();
            
            // Configure outbox settings if provided
            var outBoxConfiguration = new BonMessagingOutBoxConfiguration(configuration);
            action?.Invoke(outBoxConfiguration);
            
            return configuration;
        }
    }
}