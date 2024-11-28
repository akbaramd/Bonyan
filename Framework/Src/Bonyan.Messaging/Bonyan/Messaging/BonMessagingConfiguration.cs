using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Bonyan.Messaging.Abstractions;
using Bonyan.Modularity;

namespace Bonyan.Messaging
{
    /// <summary>
    /// Provides configuration options for registering message consumers in the messaging system.
    /// </summary>
    public class BonMessagingConfiguration
    {
        private readonly List<ConsumerRegistration> _consumerRegistrations = new List<ConsumerRegistration>();

        public BonMessagingConfiguration(BonConfigurationContext services,
            ServiceLifetime consumerLifetime = ServiceLifetime.Transient)
        {
            Context = services ?? throw new ArgumentNullException(nameof(services));
            ConsumerLifetime = consumerLifetime;
            // Register the configuration instance
            Context.Services.AddSingleton(this);
        }

        public BonConfigurationContext Context { get; set; }

        /// <summary>
        /// Gets or sets the default <see cref="ServiceLifetime"/> for registered consumers.
        /// The default is <see cref="ServiceLifetime.Transient"/>.
        /// </summary>
        public ServiceLifetime ConsumerLifetime { get; set; }

        /// <summary>
        /// Gets or sets the service name for this service. It is used for routing and identification.
        /// </summary>
        public string ServiceName { get; set; }


        /// <summary>
        /// Registers a specific consumer type with an optional queue name.
        /// </summary>
        public BonMessagingConfiguration RegisterConsumer<TConsumer>(string? queueName = null,
            ServiceLifetime? lifetime = null)
            where TConsumer : class
        {
            var consumerType = typeof(TConsumer);
            RegisterConsumerType(consumerType, queueName ?? Context.ServiceManager.ServiceId, lifetime);
            return this;
        }

        private void RegisterConsumerType(Type consumerType, string? queueName, ServiceLifetime? lifetime = null)
        {
            var interfaces = consumerType.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IBonMessageConsumer<>));

            foreach (var serviceType in interfaces)
            {
                if (!Context.Services.Any(descriptor =>
                        descriptor.ServiceType == serviceType && descriptor.ImplementationType == consumerType))
                {
                    var descriptor = new ServiceDescriptor(consumerType, consumerType, lifetime ?? ConsumerLifetime);
                    Context.Services.Add(descriptor);

                    _consumerRegistrations.Add(new ConsumerRegistration
                    {
                        ServiceType = serviceType,
                        ImplementationType = consumerType,
                        Lifetime = lifetime ?? ConsumerLifetime,
                        QueueName = queueName ?? ServiceName
                    });
                }
            }
        }

        /// <summary>
        /// Checks if the type implements the consumer interface.
        /// </summary>
        private static bool ImplementsConsumerInterface(Type type)
        {
            return type.GetInterfaces().Any(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IBonMessageConsumer<>));
        }

        /// <summary>
        /// Represents a registration of a consumer, including service type, implementation type, lifetime, and queue name.
        /// </summary>
        public class ConsumerRegistration
        {
            public Type ServiceType { get; set; }
            public Type ImplementationType { get; set; }
            public ServiceLifetime Lifetime { get; set; }
            public string QueueName { get; set; }
        }

        /// <summary>
        /// Gets the collection of consumer registrations.
        /// </summary>
        public IEnumerable<ConsumerRegistration> GetConsumerRegistrations()
        {
            return _consumerRegistrations;
        }
    }
}