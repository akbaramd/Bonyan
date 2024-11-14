using System;
using System.Linq;
using System.Reflection;
using Bonyan.Messaging;
using Bonyan.Messaging.Abstractions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BonMessagingServiceCollectionExtensions
    {
        /// <summary>
        /// Adds and configures message consumers to the service collection based on the specified options action.
        /// </summary>
        /// <param name="services">The service collection to add consumers to.</param>
        /// <param name="configureOptions">An action to configure MessagingOptions.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddBonMessaging(
            this IServiceCollection services,
            Action<BonMessagingOptions> configureOptions)
        {
            var options = new BonMessagingOptions();
            configureOptions(options);
            return services.AddBonMessaging(options);
        }

        /// <summary>
        /// Adds and configures message consumers to the service collection based on the specified options.
        /// </summary>
        /// <param name="services">The service collection to add consumers to.</param>
        /// <param name="configureOptions">An action to configure MessagingOptions.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddBonMessaging(
            this IServiceCollection services,
            BonMessagingOptions options)
        {

            services.AddSingleton<IMessageDispatcher, InMemoryMessageDispatcher>();

            options.RegisterConsumers(services);
            
            return services;
        }

        /// <summary>
        /// Registers all IMessageConsumer implementations from a given assembly.
        /// </summary>
        private static void RegisterConsumersFromAssembly(IServiceCollection services, Assembly assembly)
        {
            var consumerTypes = assembly.GetTypes()
                .Where(t => typeof(IBonMessageConsumer).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
                .ToList();

            foreach (var consumerType in consumerTypes)
            {
                services.AddTransient(typeof(IBonMessageConsumer), consumerType);
            }
        }

        /// <summary>
        /// Registers a specific IMessageConsumer type directly.
        /// </summary>
        private static void RegisterConsumer(IServiceCollection services, Type consumerType)
        {
            if (!typeof(IBonMessageConsumer).IsAssignableFrom(consumerType))
            {
                throw new ArgumentException($"Type {consumerType.Name} does not implement IMessageConsumer.");
            }

            services.AddTransient(consumerType);
        }
    }
}