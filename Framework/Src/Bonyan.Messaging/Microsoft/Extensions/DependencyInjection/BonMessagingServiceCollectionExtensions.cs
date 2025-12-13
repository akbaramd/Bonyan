using Bonyan;
using Bonyan.Mediators;
using Bonyan.Messaging;
using Bonyan.Messaging.Abstractions;
using Bonyan.Messaging.Saga;
using Bonyan.Modularity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BonMessagingServiceCollectionExtensions
    {
        /// <summary>
        /// Adds messaging services to the application with the specified configuration.
        /// </summary>
        /// <param name="context">The Bonyan configuration context.</param>
        /// <param name="configureOptions">An action to configure messaging options.</param>
        /// <returns>The updated configuration context.</returns>
        public static BonConfigurationContext AddMessaging(
            this BonConfigurationContext context,
            Action<BonMessagingConfiguration> configureOptions)
        {
            if (configureOptions == null)
                throw new ArgumentNullException(nameof(configureOptions));

            // Register mediator-based message bus as default implementation
            // This will be overridden if RabbitMQ or other implementations are registered
            context.Services.AddSingleton<IBonMessageBus>(sp =>
            {
                // Use mediator-based implementation as default
                var mediator = sp.GetRequiredService<IBonMediator>();
                var logger = sp.GetRequiredService<ILogger<MediatorBonMessageBus>>();
                return new MediatorBonMessageBus(mediator, sp, logger);
            });

            // Register wrapper handlers for mediator integration
            RegisterMediatorWrapperHandlers(context.Services);

            context.Services.AddSingleton<IBonStateStore, BonInMemoryStateStore>();
            
            context.ServiceManager.RegisterFeatureHealth("BonMessaging", () => BonHealthStatus.Up);
            
            // Create and configure messaging options
            var options = new BonMessagingConfiguration(context);
            configureOptions(options);
            
            return context;
        }

        /// <summary>
        /// Registers mediator wrapper handlers for message type adaptation.
        /// </summary>
        private static void RegisterMediatorWrapperHandlers(IServiceCollection services)
        {
            // Note: Generic handler registration will be handled dynamically by the mediator
            // when wrapper messages are created. This ensures proper type resolution.
        }
    }
}