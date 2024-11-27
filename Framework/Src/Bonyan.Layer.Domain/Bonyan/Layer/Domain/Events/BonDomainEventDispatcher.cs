using Bonyan.Layer.Domain.DomainEvent.Abstractions;
using Bonyan.Mediators;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Layer.Domain.Events;

public class BonDomainEventDispatcher(IServiceProvider serviceProvider) : IBonDomainEventDispatcher
{
    private readonly IBonMediator _bonMessageBus =
        serviceProvider.GetRequiredService<IBonMediator>();

    public Task DispatchAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default)
        where TEvent : IBonDomainEvent
    {
        return _bonMessageBus.PublishAsync(domainEvent, cancellationToken);
    }
}