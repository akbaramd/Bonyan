# AggregateRoots

This guide explains the concept of aggregate roots in **Bonyan.Layer.Domain** and how to use them effectively, including adding, clearing, and removing domain events.

## Overview
Aggregate roots are a critical part of the domain model that represent a cluster of domain objects (entities and value objects) that should be treated as a single unit for data changes. In **Bonyan.Layer.Domain**, aggregate roots are implemented to ensure consistency across related entities and to encapsulate complex business rules.

### Aggregate Root Base Classes
The **Bonyan.Layer.Domain** library provides the following base classes for aggregate roots:

- **BonAggregateRoot**: Represents the base class for aggregate roots in the domain. It is used when you do not have a strongly-typed key.
- **BonAggregateRoot<TKey>**: Represents the base class for aggregate roots with a strongly-typed key (`TKey`).

These classes inherit from `BonEntity` or `BonEntity<TKey>`, respectively, and implement additional features for managing domain events.

### Aggregate Root Interfaces
The **Bonyan.Layer.Domain** library also provides interfaces that are implemented by the aggregate root classes. These interfaces are defined in **Bonyan.Layer.Domain.Aggregate.Abstractions**:

```csharp
using Bonyan.Layer.Domain.DomainEvent.Abstractions;
using Bonyan.Layer.Domain.Entity;

namespace Bonyan.Layer.Domain.Aggregate.Abstractions;

public interface IBonAggregateRoot : IBonEntity
{
    IReadOnlyCollection<IBonDomainEvent> DomainEvents { get; }
    void ClearEvents();
}

public interface IBonAggregateRoot<TKey> : IBonAggregateRoot, IBonEntity<TKey>
{
}
```

These interfaces define the properties and methods that are common to all aggregate roots, including managing domain events.

## Usage
Aggregate roots are designed to handle domain events that are associated with the lifecycle of the aggregate. The following sections explain how to work with aggregate roots in your domain.

### Example of Defining an Aggregate Root
Below is an example of defining an aggregate root that inherits from `BonAggregateRoot<TKey>`:

```csharp
using Bonyan.Layer.Domain.Aggregate;
using Bonyan.Layer.Domain.DomainEvent.Abstractions;

public class Order : BonAggregateRoot<int>
{
    public int OrderNumber { get; set; }
    public DateTime OrderDate { get; set; }

    public void CompleteOrder()
    {
        // Business logic for completing the order
        AddDomainEvent(new OrderCompletedEvent(OrderNumber));
    }
}
```

In this example, `Order` is an aggregate root that inherits from `BonAggregateRoot<int>`, meaning it has a strongly-typed key (`int`). The `CompleteOrder` method adds a domain event (`OrderCompletedEvent`) to signify that an order has been completed.

### Managing Domain Events
Aggregate roots in **Bonyan.Layer.Domain** are equipped to manage domain events effectively. Below are the methods you can use to work with domain events:

#### 1. **Add a Domain Event**
Domain events are used to represent something significant happening in the domain. To register a domain event, use the `AddDomainEvent` method provided by the aggregate root base classes.

```csharp
protected void AddDomainEvent(IBonDomainEvent bonDomainEvent)
{
    _domainEvents.Add(bonDomainEvent);
}
```

In practice, you would call this method from within your business logic to record events that should be acted upon later:

```csharp
public void ShipOrder()
{
    // Business logic for shipping the order
    AddDomainEvent(new OrderShippedEvent(OrderNumber));
}
```

#### 2. **Clear Domain Events**
After processing the domain events, you can clear them using the `ClearDomainEvents` method.

```csharp
public void ClearDomainEvents()
{
    _domainEvents.Clear();
}
```

This is typically done after all the domain events have been handled and there is no need to keep them anymore.

#### 3. **Remove a Specific Domain Event**
If you need to remove a specific domain event from the aggregate, you can use the `RemoveDomainEvent` method:

```csharp
protected void RemoveDomainEvent(IBonDomainEvent bonDomainEvent)
{
    _domainEvents.Remove(bonDomainEvent);
}
```

This can be useful when certain conditions invalidate an event before it is handled.

### Example of Using Domain Events in a Service
You can use an aggregate root in conjunction with domain services to apply business logic and manage domain events:

```csharp
public class OrderService
{
    public void ProcessOrder(Order order)
    {
        order.CompleteOrder();
        // Handle domain events, e.g., publishing to a message queue
        foreach (var domainEvent in order.DomainEvents)
        {
            // Handle domain event
        }
        order.ClearDomainEvents();
    }
}
```

In the above example, `OrderService` processes an order and handles the domain events before clearing them.

## Summary
- **BonAggregateRoot** and **BonAggregateRoot<TKey>** are the base classes for aggregate roots in **Bonyan.Layer.Domain**, allowing you to manage domain entities and events.
- Aggregate roots are responsible for managing their domain events, including adding, clearing, and removing events.
- Domain events help communicate changes in the aggregate, making it easier to handle complex business processes and maintain consistency.

Using aggregate roots and domain events in **Bonyan.Layer.Domain** enables you to encapsulate your business logic effectively and ensure consistency across your domain model.

