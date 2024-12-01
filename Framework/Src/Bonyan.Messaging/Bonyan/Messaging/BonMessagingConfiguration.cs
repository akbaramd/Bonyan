using Microsoft.Extensions.DependencyInjection;
using Bonyan.Messaging.Abstractions;
using Bonyan.Messaging.Saga;
using Bonyan.Modularity;
using Bonyan.StateMachine;

namespace Bonyan.Messaging
{
    /// <summary>
    /// Provides configuration options for registering message consumers and sagas in the messaging system.
    /// </summary>
    public class BonMessagingConfiguration
    {
        private readonly List<ConsumerRegistration> _consumerRegistrations = new();
        private readonly List<SagaRegistration> _sagaRegistrations = new();

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

        /// <summary>
        /// Registers a specific consumer type with an optional queue name.
        /// </summary>
        public BonMessagingConfiguration RegisterConsumer<TConsumer>(ServiceLifetime? lifetime = null)
            where TConsumer : class
        {
            var consumerType = typeof(TConsumer);

            // Register the consumer type with the DI container
            if (!Context.Services.Any(descriptor => descriptor.ServiceType == consumerType))
            {
                Context.Services.Add(new ServiceDescriptor(consumerType, consumerType, lifetime ?? ConsumerLifetime));
            }

            return this;
        }

        /// <summary>
        /// Registers a specific saga type and its related consumers.
        /// </summary>
        public BonMessagingConfiguration RegisterSaga<TSaga, TInstance>(string? queueName = null,ServiceLifetime? lifetime = null)
            where TSaga : BonMessagingSagaMachine<TInstance> where TInstance : class, IStateInstance, new()
        {
            var sagaType = typeof(TSaga);

            if (!ImplementsSagaInterface(sagaType))
                throw new InvalidOperationException($"Type {sagaType.Name} does not implement a valid saga interface.");

            var descriptor = new ServiceDescriptor(sagaType, sagaType, lifetime ?? ConsumerLifetime);
            Context.Services.Add(descriptor);

            _sagaRegistrations.Add(new SagaRegistration
            {
                QueueName = queueName ?? Context.ServiceManager.ServiceId,
                SagaType = sagaType,
                Lifetime = lifetime ?? ConsumerLifetime
            });

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
                        QueueName = queueName ?? Context.ServiceManager.ServiceId
                    });
                }
            }
        }

        /// <summary>
        /// Checks if the type implements the saga interface.
        /// </summary>
        private static bool ImplementsSagaInterface(Type type)
        {
            return type.BaseType != null &&
                   type.BaseType.IsGenericType &&
                   type.BaseType.GetGenericTypeDefinition() == typeof(BonMessagingSagaMachine<>);
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
        /// Represents a registration of a saga, including its type and lifetime.
        /// </summary>
        public class SagaRegistration
        {
            public string QueueName { get; set; }
            public Type SagaType { get; set; }
            public ServiceLifetime Lifetime { get; set; }
        }

        /// <summary>
        /// Gets the collection of consumer registrations.
        /// </summary>
        public IEnumerable<ConsumerRegistration> GetConsumerRegistrations()
        {
            return _consumerRegistrations;
        }

        /// <summary>
        /// Gets the collection of saga registrations.
        /// </summary>
        public IEnumerable<SagaRegistration> GetSagaRegistrations()
        {
            return _sagaRegistrations;
        }
    }
}
