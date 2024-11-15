using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Bonyan.Messaging.Abstractions;
using Bonyan.Messaging.Attributes;

namespace Bonyan.Messaging
{
    /// <summary>
    /// Provides configuration options for registering message consumers and dispatchers in the messaging system.
    /// </summary>
    public class BonMessagingOptions
    {
        private readonly List<Assembly> _assemblies = new();
        private readonly List<Type> _consumerTypes = new();
        private readonly List<ConsumerRegistration> _consumerRegistrations = new();

        /// <summary>
        /// Gets or sets the default <see cref="ServiceLifetime"/> for registered consumers.
        /// The default is <see cref="ServiceLifetime.Transient"/>.
        /// </summary>
        public ServiceLifetime ConsumerLifetime { get; set; } = ServiceLifetime.Transient;

        /// <summary>
        /// Gets or sets a type filter predicate to include or exclude certain consumer types.
        /// By default, all types are included.
        /// </summary>
        public Func<Type, bool> TypeFilter { get; set; } = type => true;

        /// <summary>
        /// Gets or sets the service name for this service. It is used for routing and identification.
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// Gets the type of the message dispatcher to use.
        /// </summary>
        internal Type DispatcherType { get; private set; } = typeof(InMemoryBonMessageDispatcher);

        /// <summary>
        /// Sets the message dispatcher to use.
        /// </summary>
        /// <typeparam name="TDispatcher">The type of the message dispatcher.</typeparam>
        /// <returns>The current <see cref="BonMessagingOptions"/> instance for chaining.</returns>
        public BonMessagingOptions UseDispatcher<TDispatcher>() where TDispatcher : class, IBonMessageDispatcher
        {
            DispatcherType = typeof(TDispatcher);
            return this;
        }

        /// <summary>
        /// Registers all consumers that implement <see cref="IBonMessageConsumer{TMessage}"/> from the specified assemblies.
        /// </summary>
        /// <param name="assemblies">The assemblies to scan for consumer types.</param>
        /// <returns>The current <see cref="BonMessagingOptions"/> instance for chaining.</returns>
        public BonMessagingOptions RegisterConsumersFromAssemblies(params Assembly[] assemblies)
        {
            _assemblies.AddRange(assemblies);
            return this;
        }

        /// <summary>
        /// Registers specific consumer types directly.
        /// </summary>
        /// <param name="types">The consumer types to register.</param>
        /// <returns>The current <see cref="BonMessagingOptions"/> instance for chaining.</returns>
        public BonMessagingOptions RegisterConsumersFromTypes(params Type[] types)
        {
            _consumerTypes.AddRange(types);
            return this;
        }

        /// <summary>
        /// Registers a specific consumer type using a generic method.
        /// </summary>
        /// <typeparam name="TConsumer">The consumer type to register.</typeparam>
        /// <param name="lifetime">
        /// Optional service lifetime for this consumer. If not specified, the default <see cref="ConsumerLifetime"/> is used.
        /// </param>
        /// <param name="serviceName">
        /// Optional service name for this consumer. If not specified, the default <see cref="ServiceName"/> is used.
        /// </param>
        /// <returns>The current <see cref="BonMessagingOptions"/> instance for chaining.</returns>
        public BonMessagingOptions RegisterConsumer<TConsumer>(ServiceLifetime? lifetime = null, string serviceName = null)
            where TConsumer : class
        {
            var consumerType = typeof(TConsumer);

            if (!_consumerTypes.Contains(consumerType))
            {
                _consumerTypes.Add(consumerType);
            }

            // Add specific registration with custom lifetime if provided
            RegisterConsumerType(consumerType, lifetime ?? ConsumerLifetime, serviceName);

            return this;
        }

        /// <summary>
        /// Registers all discovered consumers into the provided <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The service collection to register consumers into.</param>
        internal void RegisterConsumers(IServiceCollection services)
        {
            // Collect all consumer types from assemblies
            var assemblyConsumerTypes = _assemblies
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsClass && !type.IsAbstract && ImplementsConsumerInterface(type))
                .Where(TypeFilter);

            // Register consumers from assemblies
            foreach (var consumerType in assemblyConsumerTypes)
            {
                RegisterConsumerType(consumerType, ConsumerLifetime, null);
            }

            // Register specific consumer types
            foreach (var consumerType in _consumerTypes.Where(TypeFilter))
            {
                RegisterConsumerType(consumerType, ConsumerLifetime, null);
            }

            // Register all collected consumer registrations into the IServiceCollection
            foreach (var registration in _consumerRegistrations)
            {
                var descriptor = new ServiceDescriptor(registration.ServiceType, registration.ImplementationType, registration.Lifetime);
                services.Add(descriptor);
            }
        }

        /// <summary>
        /// Registers the message dispatcher into the provided <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The service collection to register the dispatcher into.</param>
        internal void RegisterDispatcher(IServiceCollection services)
        {
            // Check if IMessageDispatcher is already registered
            if (!services.Any(service => service.ServiceType == typeof(IBonMessageDispatcher)))
            {
                services.AddSingleton(typeof(IBonMessageDispatcher), DispatcherType);
            }
        }

        /// <summary>
        /// Determines whether the specified type implements the <see cref="IBonMessageConsumer{TMessage}"/> interface.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>
        /// <see langword="true"/> if the type implements <see cref="IBonMessageConsumer{TMessage}"/>; otherwise, <see langword="false"/>.
        /// </returns>
        private static bool ImplementsConsumerInterface(Type type)
        {
            return type.GetInterfaces()
                       .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IBonMessageConsumer<>));
        }

        /// <summary>
        /// Registers a specific consumer type into the collection of consumer registrations.
        /// </summary>
        /// <param name="consumerType">The consumer type to register.</param>
        /// <param name="lifetime">The service lifetime for the consumer.</param>
        /// <param name="serviceName">
        /// The service name for the consumer. If null, attempts to read from the <see cref="ConsumerServiceAttribute"/>,
        /// or uses the default <see cref="ServiceName"/>.
        /// </param>
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
                if (!_consumerRegistrations.Any(reg => reg.ServiceType == serviceType && reg.ImplementationType == consumerType))
                {
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
