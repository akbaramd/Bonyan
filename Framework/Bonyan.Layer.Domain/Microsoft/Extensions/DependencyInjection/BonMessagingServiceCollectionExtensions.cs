using System;
using System.Linq;
using System.Reflection;
using Bonyan.Layer.Domain;
using Bonyan.Layer.Domain.DomainEvent.Abstractions;
using Bonyan.Layer.Domain.Events;
using Bonyan.Messaging;
using Bonyan.Messaging.Abstractions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BonDomainServiceCollectionExtensions
    {
        /// <summary>
        /// Adds and configures message consumers to the service collection based on the specified options action.
        /// </summary>
        /// <param name="services">The service collection to add consumers to.</param>
        /// <param name="configureOptions">An action to configure MessagingOptions.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddBonDomainLayer(
            this IServiceCollection services
            )
        {
            services.AddSingleton<IBonDomainEventDispatcher, BonDomainEventDispatcher>();
            return services;
        }

    }
}