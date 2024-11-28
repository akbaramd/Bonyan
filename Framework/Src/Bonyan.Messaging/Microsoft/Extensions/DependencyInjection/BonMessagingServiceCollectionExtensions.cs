using System;
using Bonyan;
using Microsoft.Extensions.DependencyInjection;
using Bonyan.Messaging;
using Bonyan.Messaging.Abstractions;
using Bonyan.Modularity;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BonMessagingServiceCollectionExtensions
    {
        /// <summary>
        /// Adds messaging services to the application with the specified configuration.
        /// </summary>
        /// <param name="context">The Bonyan configuration context.</param>
        /// <param name="serviceName">The name of the service for identification and routing.</param>
        /// <param name="configureOptions">An action to configure messaging options.</param>
        /// <returns>The updated configuration context.</returns>
        public static BonConfigurationContext AddMessaging(
            this BonConfigurationContext context,
            Action<BonMessagingConfiguration> configureOptions)
        {
            if (configureOptions == null)
                throw new ArgumentNullException(nameof(configureOptions));

            // Create and configure messaging options
            var options = new BonMessagingConfiguration(context);

            // Ensure the message bus is implemented
            context.Services.AddTransient<IBonMessageBus>(sp =>
                throw new NotImplementedException(
                    "No message bus implementation provided. Please register a compatible message bus (e.g., RabbitMQ, Azure Service Bus, etc.)."));

            configureOptions(options);

            context.ServiceManager.RegisterFeatureHealth("BonMessaging", () => BonHealthStatus.Up);

            return context;
        }
    }
}