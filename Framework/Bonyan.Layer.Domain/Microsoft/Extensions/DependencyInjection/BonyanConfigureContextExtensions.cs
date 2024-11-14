﻿using Bonyan.Layer.Domain.Abstractions;
using Bonyan.Layer.Domain.Events;
using Bonyan.Modularity;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BonyanConfigureContextExtensions
    {
        /// <summary>
        /// Registers a domain handler of type <typeparamref name="THandler"/> for events of type <typeparamref name="TEvent"/> as a transient service.
        /// </summary>
        /// <typeparam name="THandler">The type of the domain handler to register.</typeparam>
        /// <typeparam name="TEvent">The type of the domain event the handler handles.</typeparam>
        /// <param name="bon">The service collection to add the domain handler to.</param>
        /// <returns>The updated service collection.</returns>
        public static BonConfigurationContext AddDomainHandler<THandler, TEvent>(this BonConfigurationContext bon)
            where THandler : class, IBonDomainEventHandler<TEvent>
            where TEvent : IBonDomainEvent
        {
            bon.Services.AddDomainHandler<THandler, TEvent>();
            return bon;
        }

        /// <summary>
        /// Registers all domain handlers from the specified type's assembly as transient services.
        /// </summary>
        /// <param name="bon">The service collection to add the domain handlers to.</param>
        /// <param name="handlerType">A type whose assembly will be scanned for domain handlers.</param>
        /// <returns>The updated service collection.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="handlerType"/> is null.</exception>
        public static BonConfigurationContext AddDomainHandler(this BonConfigurationContext bon, Type handlerType)
        {
            bon.Services.AddDomainHandler(handlerType);
            return bon;
        }
    }
}