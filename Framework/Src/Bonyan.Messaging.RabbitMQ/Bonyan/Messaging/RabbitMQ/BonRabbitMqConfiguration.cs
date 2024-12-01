using Bonyan.Messaging.Saga;
using Bonyan.StateMachine;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Messaging.RabbitMQ
{
    public class BonRabbitMqConfiguration
    {
        private readonly RabbitMQCosnumerTypeAccessor _rabbitMqCosnumerTypeAccessor;
        private readonly RabbitMQSagaTypeAccessor _rabbitMqSagaTypeAccessor;

        public BonRabbitMqConfiguration(BonMessagingConfiguration configuration)
        {
            Configuration = configuration;
            _rabbitMqCosnumerTypeAccessor = new RabbitMQCosnumerTypeAccessor();
            _rabbitMqSagaTypeAccessor = new RabbitMQSagaTypeAccessor();
            Configuration.Context.Services.AddSingleton(_rabbitMqCosnumerTypeAccessor);
            Configuration.Context.Services.AddSingleton(_rabbitMqSagaTypeAccessor);
        }

        public BonMessagingConfiguration Configuration { get; set; }
        public string HostName { get; set; } = "localhost";
        public int Port { get; set; } = 5672;
        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public string VirtualHost { get; set; } = "/";

        /// <summary>
        /// Configures a consumer and maps it to a specific queue.
        /// </summary>
        public void ConfigureConsumer<TConsumer>(string? queueName = null)
            where TConsumer : class
        {
            queueName ??= $"{Configuration.Context.ServiceManager.ServiceId}.{typeof(TConsumer).Name}" ;
            if (string.IsNullOrWhiteSpace(queueName))
                throw new ArgumentException("Queue name cannot be null or empty.", nameof(queueName));

            var consumerType = typeof(TConsumer);

            // Add to the accessor for later use
            _rabbitMqCosnumerTypeAccessor.AddMapping(consumerType, queueName);
        }

        /// <summary>
        /// Configures a saga and maps it to a specific queue.
        /// </summary>
        public void ConfigureSaga<TSaga, TInstance>(string? queueName = null)
            where TSaga : BonMessagingSagaMachine<TInstance>
            where TInstance : class, IStateInstance, new()
        {
            queueName ??= $"{Configuration.Context.ServiceManager.ServiceId}.{typeof(TSaga).Name}" ;
            if (string.IsNullOrWhiteSpace(queueName))
                throw new ArgumentException("Queue name cannot be null or empty.", nameof(queueName));

            var sagaType = typeof(TSaga);

            if (!ImplementsSagaInterface(sagaType))
                throw new InvalidOperationException($"Type {sagaType.Name} does not implement a valid saga interface.");

            // Add to the saga accessor
            _rabbitMqSagaTypeAccessor.AddSaga(sagaType, typeof(TInstance), queueName);
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
    }
}