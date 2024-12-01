using Bonyan.Messaging;
using Bonyan.Messaging.Abstractions;
using Bonyan.Messaging.OutBox;
using Bonyan.Messaging.OutBox.HostedServices;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BonMessagingOutboxServiceCollectionExtensions
    {
        public static BonMessagingConfiguration AddOutbox(
            this BonMessagingConfiguration configuration, Action<BonMessagingOutBoxConfiguration>? action = null )
        {
            configuration.Context.Services.AddSingleton<IOutboxStore, InMemoryOutboxStore>();
            configuration.Context.Services.Replace(ServiceDescriptor.Singleton<IBonMessageBus, BonOutBoxMessageBox>());

            configuration.Context.Services.AddHostedService<OutboxProcessorHostedService>();
            var outBoxConfiguration = new BonMessagingOutBoxConfiguration(configuration);
            action?.Invoke(outBoxConfiguration);
            return configuration;
        }
    }
}