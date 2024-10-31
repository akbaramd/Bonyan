﻿using System;
using System.Linq;
using System.Reflection;
using Bonyan.Layer.Domain.Events;
using Bonyan.Modularity;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BonyanConfigureContextExtensions
    {
        /// <summary>
        /// Registers a domain handler of type <typeparamref name="THandler"/> for events of type <typeparamref name="TEvent"/> as a transient service.
        /// </summary>
        /// <typeparam name="THandler">The type of the domain handler to register.</typeparam>
        /// <typeparam name="TEvent">The type of the domain event the handler handles.</typeparam>
        /// <param name="services">The service collection to add the domain handler to.</param>
        /// <returns>The updated service collection.</returns>
        public static ServiceConfigurationContext AddDomainHandler<THandler, TEvent>(this ServiceConfigurationContext services)
            where THandler : class, IDomainEventHandler<TEvent>
            where TEvent : IDomainEvent
        {
            services.Services.AddTransient<IDomainEventHandler<TEvent>, THandler>();
            return services;
        }

        /// <summary>
        /// Registers all domain handlers from the specified type's assembly as transient services.
        /// </summary>
        /// <param name="services">The service collection to add the domain handlers to.</param>
        /// <param name="handlerType">A type whose assembly will be scanned for domain handlers.</param>
        /// <returns>The updated service collection.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="handlerType"/> is null.</exception>
        public static ServiceConfigurationContext AddDomainHandler(this ServiceConfigurationContext services, Type handlerType)
        {
            if (handlerType == null)
            {
                throw new ArgumentNullException(nameof(handlerType));
            }

            // Get the assembly from the specified type
            var assembly = handlerType.Assembly;

            // Find all types in the assembly that implement IDomainEventHandler<TEvent>
            var handlerTypes = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract)
                .SelectMany(t => t.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>))
                    .Select(i => new { Handler = t, Interface = i }))
                .ToList();

            // Register each handler type with its corresponding IDomainEventHandler<TEvent> interface
            foreach (var handler in handlerTypes)
            {
                services.Services.AddTransient(handler.Interface, handler.Handler);
            }

            return services;
        }
    }
}