using Bonyan.Messaging.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Messaging
{
    public class InMemoryMessageDispatcher(IServiceProvider serviceProvider) : IMessageDispatcher
    {
        public async Task SendAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
            where TMessage : IBonMessage
        {
            // Get a single consumer for this message type
            var consumer = serviceProvider.GetService<IBonMessageConsumer<TMessage>>();
            if (consumer != null)
            {
                await consumer.ConsumeAsync(message, cancellationToken);
            }
        }

        public async Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
            where TMessage : IBonMessage
        {
            // Get all consumers for this message type
            var consumers = serviceProvider.GetServices<IBonMessageConsumer<TMessage>>().ToList();

            var tasks = consumers.Select(consumer => consumer.ConsumeAsync(message, cancellationToken)).ToList();

            await Task.WhenAll(tasks);
        }
    }
}