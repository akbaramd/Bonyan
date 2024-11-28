using Bonyan.Messaging;
using Bonyan.Messaging.Abstractions;
using Bonyan.Messaging.RabbitMQ;
using Bonyan.Modularity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BonMessagingRabbitMqServiceCollectionExtensions
    {
        public static BonMessagingConfiguration AddRabbitMQ(
            this BonMessagingConfiguration configuration,
            Action<RabbitMQOptions> configureOptions)
        {
            configuration.Context.ConfigureOptions(configureOptions);
            configuration.Context.Services.Replace(ServiceDescriptor.Transient<IBonMessageBus,RabbitMQMessageBus>()) ;

            return configuration;
        }
  
    }
}