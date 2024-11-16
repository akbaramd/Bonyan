# Domain Events in Bonyan.Layer.Domain

This guide explains the concept of domain events in **Bonyan.Layer.Domain** and how they can be used effectively to communicate between aggregates and maintain a clean, decoupled architecture.

## Overview
Domain events are an important part of domain-driven design (DDD). They represent significant occurrences within the domain that other parts of the system may be interested in. In **Bonyan.Layer.Domain**, domain events help maintain decoupling between aggregates by providing a way to react to important state changes without tight coupling.

### Messaging Integration
The **BonDomainEventBase** class and related interfaces use the messaging module in **Bonyan.Layer.Domain** to support the communication of domain events. To use domain events effectively, you must integrate them with the **Bonyan Messaging Module**. This ensures that domain events can be dispatched, handled, and processed consistently across the system.

For more information, refer to the [Bonyan Messaging Module Documentation](https://docs.bonyan.com/messaging-module).

## BonDomainEvent Interface
The **BonDomainEvent** interface is used to represent domain events. It extends **IBonMessage** to support integration with the messaging system, enabling domain events to be treated as messages that can be dispatched to subscribers.

```csharp
using Bonyan.Messaging.Abstractions;

namespace Bonyan.Layer.Domain.DomainEvent.Abstractions;

public interface IBonDomainEvent : IBonMessage
{
}
```

This interface serves as the base for all domain events, ensuring that they can be easily integrated into the messaging infrastructure.

## BonDomainEventDispatcher Interface
The **BonDomainEventDispatcher** is responsible for dispatching domain events from aggregates. It provides an asynchronous method to send events and clear them once dispatched.

```csharp
namespace Bonyan.Layer.Domain.DomainEvent.Abstractions;

/// <summary>
///     Interface for domain event dispatchers, responsible for dispatching and clearing domain events from aggregates.
/// </summary>
public interface IBonDomainEventDispatcher
{
    Task DispatchAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default) where TEvent : IBonDomainEvent;
}
```

This dispatcher uses the **Message Dispatcher** from the messaging module to broadcast events that occur within the domain, ensuring that subscribers are notified of important changes. Therefore, it is essential to add the messaging module to fully utilize the domain event capabilities.

## BonDomainEventBase Class
The **BonDomainEventBase** class provides a simple base for defining domain events, with properties like `DateOccurred` to indicate when the event occurred.

```csharp
using Bonyan.Layer.Domain.DomainEvent.Abstractions;

namespace Bonyan.Layer.Domain.Events;

public abstract class BonDomainEventBase : IBonDomainEvent
{
    protected BonDomainEventBase()
    {
        DateOccurred = DateTime.UtcNow;
    }

    public DateTime DateOccurred { get; protected set; } = DateTime.UtcNow;
}
```

## Using Domain Events with Aggregate Roots
Aggregate roots in **Bonyan.Layer.Domain** can use domain events to maintain a clean architecture by emitting events whenever significant changes occur. Below is an example of how to implement and use domain events in an aggregate root.

### Example: Aggregate Root with Domain Events
Consider the following aggregate root that raises a domain event whenever a customer is created:

```csharp
using Bonyan.Layer.Domain.Aggregate;
using Bonyan.Layer.Domain.Events;
using Bonyan.Layer.Domain.DomainEvent.Abstractions;

namespace Bonyan.Layer.Domain.Aggregate;

public class CustomerAggregate : BonAggregateRoot
{
    public string Name { get; private set; }
    public string Email { get; private set; }

    public CustomerAggregate(string name, string email)
    {
        Name = name;
        Email = email;
        AddDomainEvent(new CustomerCreatedEvent(name, email));
    }
}

public class CustomerCreatedEvent : BonDomainEventBase
{
    public string Name { get; }
    public string Email { get; }

    public CustomerCreatedEvent(string name, string email)
    {
        Name = name;
        Email = email;
    }
}
```

In this example:
- **CustomerAggregate**: Represents an aggregate root that raises a `CustomerCreatedEvent` when a new customer is created.
- **AddDomainEvent**: This method adds the event to the aggregate’s list of domain events, which can then be dispatched by the **IBonDomainEventDispatcher**.

## Dispatching Domain Events
Domain events should be dispatched by using the **IBonDomainEventDispatcher**. This can typically be done within a domain service or as part of a unit of work that ensures all events are dispatched after the aggregate changes are persisted.

### Example: Dispatching Domain Events
Below is an example of a domain service that dispatches the domain events after the aggregate state has changed:

```csharp
using Bonyan.Layer.Domain.DomainService;
using Bonyan.Layer.Domain.DomainEvent.Abstractions;

public class CustomerDomainService : BonDomainService
{
    public async Task<BonDomainResult> CreateCustomerAsync(string name, string email, CancellationToken cancellationToken = default)
    {
        var customer = new CustomerAggregate(name, email);
        Logger.LogInformation("Customer created: {Name}, {Email}", name, email);

        if (DomainEventDispatcher != null)
        {
            foreach (var domainEvent in customer.DomainEvents)
            {
                await DomainEventDispatcher.DispatchAsync(domainEvent, cancellationToken);
            }
            customer.ClearDomainEvents();
        }

        return BonDomainResult.Success();
    }
}
```

### Explanation
- **CustomerDomainService**: A domain service that handles the creation of a customer.
- **Domain Event Dispatching**: After the customer is created, the **DomainEventDispatcher** is used to dispatch all domain events related to the customer aggregate.
- **ClearDomainEvents**: Clears the domain events after they are dispatched, ensuring that events are not dispatched multiple times.

## Summary
- **Domain Events**: Represent significant occurrences within the domain that other parts of the system may need to react to.
- **BonDomainEventBase**: Base class for all domain events, providing a standard structure for domain events.
- **Domain Event Dispatcher**: The **IBonDomainEventDispatcher** interface is used to dispatch domain events asynchronously.
- **Aggregate Integration**: Domain events can be added to aggregates, and dispatched as part of the workflow in a domain service.
- **Messaging Integration**: Domain events extend **IBonMessage**, making them compatible with the messaging infrastructure in **Bonyan.Layer.Domain**. Ensure to configure the messaging module properly to handle domain events across services and boundaries.

Domain events in **Bonyan.Layer.Domain** provide a powerful mechanism for ensuring that different parts of the system remain decoupled while still being able to react to changes in an efficient and consistent manner.

