using Bonyan.Messaging;
using Bonyan.Messaging.Abstractions;
using Bonyan.Messaging.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BonMessagingRabbitMqServiceCollectionExtensions
    {
        public static BonMessagingConfiguration AddRabbitMqMessaging(
            this BonMessagingConfiguration configuration,
            Action<RabbitMQOptions> configureOptions)
        {
            var options = new RabbitMQOptions();
            configureOptions(options);

            configuration.Context.Services.AddSingleton(options);
            configuration.Context.Services.Replace(ServiceDescriptor.Transient<IBonMessageBus,BonRabbitMqMessageBus>()) ;

            return configuration;
        }
    }
}