using Bonyan.Messaging;
using Bonyan.Messaging.Abstractions;
using Bonyan.Messaging.RabbitMQ;
using Bonyan.Messaging.RabbitMQ.HostedServices;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RabbitMQ.Client;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BonMessagingRabbitMqServiceCollectionExtensions
    {
        public static BonMessagingConfiguration AddRabbitMq(
            this BonMessagingConfiguration configuration,
            Action<BonRabbitMqConfiguration> configureOptions)
        {
            var configure = new BonRabbitMqConfiguration(configuration);


            var factory = new ConnectionFactory
            {
                HostName = configure.HostName,
                Port = configure.Port,
                UserName = configure.UserName,
                Password = configure.Password,
                VirtualHost = configure.VirtualHost,
                DispatchConsumersAsync = true
            };

            var connection = factory.CreateConnection();

            var channel = connection.CreateModel();

            // Declare default exchanges
            channel.ExchangeDeclare(BonMessagingConst.DefaultExchangeName, ExchangeType.Topic, durable: true);
            
            configuration.Context.Services.AddSingleton<IConnection>(connection);
            configuration.Context.Services.Replace(ServiceDescriptor
                .Singleton<IBonMessageProducer, RabbitMqProducer>());
            configuration.Context.Services.Replace(ServiceDescriptor
                .Singleton<IBonMessageSubscriber, RabbitMqSubscriber>());
            
            configuration.Context.Services.Replace(ServiceDescriptor.Singleton<IBonMessageBus, RabbitMqBus>());

            configuration.Context.Services.AddSingleton<IBonRabbitMqBus,RabbitMqBus>();
            configuration.Context.Services.AddSingleton<IBonRabbitMqProducer,RabbitMqProducer>();
            configuration.Context.Services.AddSingleton<IBonRabbitMqSubscriber,RabbitMqSubscriber>();
            
            configuration.Context.Services.AddHostedService<RabbitMqConsumerBackgroundService>();
            configuration.Context.Services.AddHostedService<RabbitMqSagaBackgroundService>();

            configureOptions.Invoke(configure);

            return configuration;
        }
    }
}