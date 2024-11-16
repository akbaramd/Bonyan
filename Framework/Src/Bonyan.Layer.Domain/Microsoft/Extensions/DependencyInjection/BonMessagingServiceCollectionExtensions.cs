using Bonyan.Exceptions;
using Bonyan.Layer.Domain.DomainEvent.Abstractions;
using Bonyan.Layer.Domain.Events;
using Bonyan.Messaging.Abstractions;
using Bonyan.Modularity;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BonDomainServiceCollectionExtensions
    {
        public static BonConfigurationContext AddDomainLayer(this BonConfigurationContext context)
        {
            // Check if the messaging layer has been added
            if (!IsMessagingLayerRegistered(context.Services))
            {
                throw new MessagingNotConfiguredException();
            }

            // Register the in-memory domain event dispatcher by default
            context.Services.AddSingleton<IBonDomainEventDispatcher, BonDomainEventDispatcher>();

            return context;
        }

        private static bool IsMessagingLayerRegistered(IServiceCollection services)
        {
            // Check if any service related to the messaging layer has been registered
            // In this case, we check for the dispatcher service to verify if messaging is set up
            return services.Any(service => service.ServiceType == typeof(IBonMessageDispatcher));
        }
    }
}