using Bonyan.Layer.Domain.DomainEvent.Abstractions;
using Bonyan.Messaging;
using Bonyan.Messaging.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Layer.Domain.Events;

public class BonDomainEventDispatcher(IServiceProvider serviceProvider) : IBonDomainEventDispatcher
{
    private readonly IBonMessageDispatcher _bonMessageDispatcher = serviceProvider.GetService<IBonMessageDispatcher>() ?? new InMemoryBonMessageDispatcher(serviceProvider);

    public Task DispatchAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default) where TEvent : IBonDomainEvent
    {
        return _bonMessageDispatcher.PublishAsync(domainEvent, cancellationToken);
    }
}