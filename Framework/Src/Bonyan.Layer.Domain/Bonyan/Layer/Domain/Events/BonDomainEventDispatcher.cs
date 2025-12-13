using Bonyan.Layer.Domain.DomainEvent.Abstractions;
using Bonyan.Mediators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Bonyan.Layer.Domain.Events;

/// <summary>
/// Domain Event Dispatcher implementation that uses Mediator pattern.
/// This is an infrastructure concern - Domain Layer itself doesn't depend on Mediator.
/// If Mediator is not available, events are silently ignored (no-op).
/// </summary>
public class BonDomainEventDispatcher : IBonDomainEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BonDomainEventDispatcher>? _logger;

    public BonDomainEventDispatcher(IServiceProvider serviceProvider, ILogger<BonDomainEventDispatcher>? logger = null)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public Task DispatchAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default)
        where TEvent : IBonDomainEvent
    {
        // Try to get Mediator - if not available, silently ignore (Domain Layer independence)
        var mediator = _serviceProvider.GetService<IBonMediator>();
        if (mediator != null)
        {
            return mediator.PublishAsync(domainEvent, cancellationToken);
        }
        
        // If Mediator is not registered, log and continue (no-op)
        // This allows Domain Layer to work independently
        _logger?.LogDebug(
            "Domain event {EventType} dispatched but no IBonMediator is registered. " +
            "Event will be ignored. Register BonMediatorModule to enable event dispatching.",
            typeof(TEvent).Name);
        
        return Task.CompletedTask;
    }
}