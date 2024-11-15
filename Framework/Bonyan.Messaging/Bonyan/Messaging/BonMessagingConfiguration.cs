using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Bonyan.Messaging.Abstractions;
using Bonyan.Messaging.Attributes;
using Bonyan.Modularity;

namespace Bonyan.Messaging
{
    /// <summary>
    /// Provides configuration options for registering message consumers and dispatchers in the messaging system.
    /// </summary>
    public class BonMessagingConfiguration
    {
        public BonMessagingConfiguration(BonConfigurationContext services, string serviceName, ServiceLifetime consumerLifetime = ServiceLifetime.Transient)
        {
            Context = services ?? throw new ArgumentNullException(nameof(services));
            ConsumerLifetime = consumerLifetime;
            ServiceName = serviceName ?? throw new ArgumentNullException(nameof(serviceName));

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
        /// Gets or sets a type filter predicate to include or exclude certain consumer types.
        /// By default, all types are included.
        /// </summary>
        public Func<Type, bool> TypeFilter { get; set; } = type => true;

        /// <summary>
        /// Gets or sets the service name for this service. It is used for routing and identification.
        /// </summary>
        public string ServiceName { get; set; }


        private readonly List<ConsumerRegistration> _consumerRegistrations = new List<ConsumerRegistration>();

        /// <summary>
        /// Registers all consumers that implement <see cref="IBonMessageConsumer{TMessage}"/> from the specified assemblies.
        /// </summary>
        public BonMessagingConfiguration RegisterConsumersFromAssemblies(params Assembly[] assemblies)
        {
            if (assemblies == null) throw new ArgumentNullException(nameof(assemblies));

            // Collect all consumer types from assemblies and register them immediately
            var consumerTypes = assemblies
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsClass && !type.IsAbstract && ImplementsConsumerInterface(type))
                .Where(TypeFilter);

            foreach (var consumerType in consumerTypes)
            {
                RegisterConsumerType(consumerType, ConsumerLifetime, null);
            }

            return this;
        }

        /// <summary>
        /// Registers specific consumer types directly.
        /// </summary>
        public BonMessagingConfiguration RegisterConsumersFromTypes(params Type[] types)
        {
            if (types == null) throw new ArgumentNullException(nameof(types));

            foreach (var consumerType in types.Where(TypeFilter))
            {
                RegisterConsumerType(consumerType, ConsumerLifetime, null);
            }

            return this;
        }

        /// <summary>
        /// Registers a specific consumer type using a generic method.
        /// </summary>
        public BonMessagingConfiguration RegisterConsumer<TConsumer>(ServiceLifetime? lifetime = null, string serviceName = null)
            where TConsumer : class
        {
            var consumerType = typeof(TConsumer);

            // Register the consumer immediately
            RegisterConsumerType(consumerType, lifetime ?? ConsumerLifetime, serviceName);

            return this;
        }


        /// <summary>
        /// Determines whether the specified type implements the <see cref="IBonMessageConsumer{TMessage}"/> interface.
        /// </summary>
        private static bool ImplementsConsumerInterface(Type type)
        {
            return type.GetInterfaces()
                       .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IBonMessageConsumer<>));
        }

        /// <summary>
        /// Registers a specific consumer type into the IServiceCollection and stores it in the collection.
        /// </summary>
        private void RegisterConsumerType(Type consumerType, ServiceLifetime lifetime, string serviceName)
        {
            var interfaces = consumerType.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IBonMessageConsumer<>));

            // Read the service name from the attribute if not provided
            if (string.IsNullOrEmpty(serviceName))
            {
                var attribute = consumerType.GetCustomAttribute<ConsumerServiceAttribute>();
                serviceName = attribute?.ServiceName ?? ServiceName;
            }

            foreach (var serviceType in interfaces)
            {
                // Avoid duplicate registrations
                if (!Context.Services.Any(descriptor => descriptor.ServiceType == serviceType && descriptor.ImplementationType == consumerType))
                {
                    // Register the consumer in the DI container
                    var descriptor = new ServiceDescriptor(serviceType, consumerType, lifetime);
                    Context.Services.Add(descriptor);

                    // Store the consumer registration
                    _consumerRegistrations.Add(new ConsumerRegistration
                    {
                        ServiceType = serviceType,
                        ImplementationType = consumerType,
                        Lifetime = lifetime,
                        ServiceName = serviceName
                    });
                }
            }
        }

        /// <summary>
        /// Represents a registration of a consumer, including service type, implementation type, lifetime, and service name.
        /// </summary>
        internal class ConsumerRegistration
        {
            public Type ServiceType { get; set; }
            public Type ImplementationType { get; set; }
            public ServiceLifetime Lifetime { get; set; }
            public string ServiceName { get; set; }
        }

        /// <summary>
        /// Gets the collection of consumer registrations.
        /// </summary>
        internal IEnumerable<ConsumerRegistration> GetConsumerRegistrations()
        {
            return _consumerRegistrations;
        }
    }
}
