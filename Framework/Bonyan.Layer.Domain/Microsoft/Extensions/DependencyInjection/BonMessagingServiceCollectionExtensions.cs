using System;
using Bonyan.Layer.Domain.DomainEvent.Abstractions;
using Bonyan.Layer.Domain.Events;
using Bonyan.Messaging.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BonDomainServiceCollectionExtensions
    {
        public static IServiceCollection AddBonDomainLayer(this IServiceCollection services)
        {
            // Check if the messaging layer has been added
            if (!IsMessagingLayerRegistered(services))
            {
                // Print a message to the console to guide the user to configure the messaging layer
                Console.WriteLine(
                    "Warning: The messaging layer is not configured. To handle domain events, please configure messaging.");
                Console.WriteLine(
                    "You can add messaging by calling services.AddBonMessaging(serviceName,messigingOPtions=>{}) in your ConfigureServices method.");

                return services;
            }

            // Register the in-memory domain event dispatcher by default
            services.AddSingleton<IBonDomainEventDispatcher, BonDomainEventDispatcher>();

            return services;
        }

        private static bool IsMessagingLayerRegistered(IServiceCollection services)
        {
            // Check if any service related to the messaging layer has been registered
            // In this case, we check for the dispatcher service to verify if messaging is set up
            return services.Any(service => service.ServiceType == typeof(IBonMessageDispatcher));
        }
    }
}