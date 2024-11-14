using Bonyan.Layer.Domain.DomainEvent.Abstractions;
using Bonyan.Messaging;

namespace Bonyan.Layer.Domain.Events
{
    internal class BonDomainEventDispatcher : IBonDomainEventDispatcher
    {
        private readonly IMessageDispatcher _messageDispatcher;

        public BonDomainEventDispatcher(IMessageDispatcher messageDispatcher)
        {
            _messageDispatcher = messageDispatcher;
        }

        public Task DispatchAsync<TDomainEvent>(TDomainEvent domainEvent, CancellationToken cancellationToken = default)
            where TDomainEvent : IBonDomainEvent
        {
            return _messageDispatcher.PublishAsync(domainEvent, cancellationToken);
        }

        
    }
}