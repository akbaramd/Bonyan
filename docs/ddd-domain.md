# Domain Module Guide
The `Bonyan.Layer.Domain` module is designed as part of a Domain-Driven Design (DDD) software architecture. This module includes various components that facilitate the management and processing of domain logic and entities within an application. Below, we will discuss the core concepts such as Entities, Value Objects, Aggregate Roots, and Enumerations, and provide examples of their usage.

```bash
dotnet add package Bonyan.Layer.Domain
```
## Adding the Domain Module

To use this module, you need to add `BonyanLayerDomainModule` to the target module that requires domain features:

```csharp
public class YourTargetModule : Module
{
    public YourTargetModule()
    {
        DependOn<BonyanLayerDomainModule>();
    }
}
```

By adding `BonyanLayerDomainModule`, all domain-related functionalities are integrated into your target module, allowing you to utilize the components described below.

## Entities

An **Entity** is a core concept in DDD that represents an object in the domain that has a unique identity. Entities usually contain both state and behavior, and they form the building blocks of the domain model. In `Bonyan.Layer.Domain`, entities can either have an explicit identifier or be identifier-less, depending on the use case.

### Example: Defining an Entity

```csharp
public class Product : Entity<int>
{
    public string Name { get; private set; }
    public decimal Price { get; private set; }

    public Product(int id, string name, decimal price)
    {
        Id = id;
        Name = name;
        Price = price;
    }
}
```
In this example, `Product` is an entity with a unique identifier of type `int`. It contains properties like `Name` and `Price` that define its state.

### Example: Entity without Identifier

```csharp
public class TemporarySession : Entity
{
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }

    public TemporarySession(DateTime startTime, DateTime endTime)
    {
        StartTime = startTime;
        EndTime = endTime;
    }
    
    // define keys of entity in  Entity without Identifier 
    public override object[] GetKeys() {
        return [ StartTime, EndTime];    
    }
}
```
Here, `TemporarySession` is an entity without an explicit identifier. This type of entity is used for temporary data that does not need a unique identity beyond its runtime context.

## Value Objects

A **Value Object** is another fundamental concept in DDD. Unlike entities, value objects do not have a unique identity and are defined by their attributes. They are immutable and are often used to represent simple domain concepts like money, dates, or measurement units.

### Example: Defining a Value Object

```csharp
public class Money : ValueObject
{
    public decimal Amount { get; private set; }
    public string Currency { get; private set; }

    public Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }
}
```
In this example, `Money` is a value object representing an amount in a specific currency. Since value objects are compared based on their values, the `GetEqualityComponents()` method is used to determine equality by comparing `Amount` and `Currency`.

## Aggregate Root

An **Aggregate Root** is an entity that serves as the main entry point to an aggregate, which is a group of related entities that are treated as a single unit. The aggregate root is responsible for maintaining the consistency and integrity of the entire aggregate.

### Role of an Aggregate Root

The main role of an aggregate root is to enforce business rules and invariants for all entities within the aggregate. This means that any change to entities within the aggregate must go through the aggregate root to ensure that all business rules are respected. Additionally, aggregate roots can manage domain events to capture significant business operations or state changes that need to be communicated across the system.

### Example: Defining an Aggregate Root

```csharp
public class Order : AggregateRoot<int>
{
    private readonly List<OrderItem> _orderItems;
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

    public Order(int id)
    {
        Id = id;
        _orderItems = new List<OrderItem>();
    }

    public void AddOrderItem(Product product, int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException("Quantity must be greater than zero.");
        }
        _orderItems.Add(new OrderItem(product, quantity));
        AddDomainEvent(new OrderItemAddedEvent(this, product, quantity));
    }
}

public class OrderItem : Entity
{
    public Product Product { get; private set; }
    public int Quantity { get; private set; }

    public OrderItem(Product product, int quantity)
    {
        Product = product;
        Quantity = quantity;
    }
}
```
In this example, `Order` is an aggregate root that manages a collection of `OrderItem` entities. The `AddOrderItem` method enforces a rule that the quantity must be greater than zero, thereby maintaining consistency within the aggregate. Additionally, it registers a domain event (`OrderItemAddedEvent`) to indicate that an item has been added, which can be used to trigger other actions within the system. Any modifications to `OrderItem` must go through the `Order` aggregate root, ensuring that the business logic is centralized.

## Different Types of Aggregate Roots

### 1. **CreationAuditableAggregateRoot**

A **Creation Auditable Aggregate Root** extends the aggregate root by adding properties to track when the entity was created. This helps in maintaining an audit trail of when entities are introduced to the system.

```csharp
public abstract class CreationAuditableAggregateRoot : AggregateRoot
{
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}
```

### 2. **ModificationAuditableAggregateRoot**

A **Modification Auditable Aggregate Root** builds upon the creation auditable root by also tracking modifications to the entity. It includes a `ModifiedDate` property to record the last update time.

```csharp
public abstract class ModificationAuditableAggregateRoot : CreationAuditableAggregateRoot
{
    public DateTime? ModifiedDate { get; set; } = DateTime.UtcNow;
}
```

### 3. **FullAuditableAggregateRoot**

A **Full Auditable Aggregate Root** extends the aggregate root concept by including auditing properties, such as tracking when the entity was created, last updated, and deleted (soft delete). This is useful for maintaining a complete history of changes to critical business entities.

```csharp
public abstract class FullAuditableAggregateRoot : ModificationAuditableAggregateRoot
{
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedDate { get; private set; }

    public void SoftDelete()
    {
        if (!IsDeleted)
        {
            IsDeleted = true;
            DeletedDate = DateTime.UtcNow;
        }
    }

    public void Restore()
    {
        if (IsDeleted)
        {
            IsDeleted = false;
            DeletedDate = null;
        }
    }
}
```

In this example, `FullAuditableAggregateRoot` provides properties for tracking creation, modification, and deletion events, allowing the entity to be "soft deleted" and later restored if needed.

## Enumeration

An **Enumeration** is a type-safe way to represent a set of named values, similar to an enum but with more capabilities and flexibility. In `Bonyan.Layer.Domain`, the `Enumeration` base class allows for creating strongly-typed enums with additional utility methods to enhance functionality.

### Example: Defining an Enumeration

```csharp
public class OrderStatus : Enumeration
{
    public static readonly OrderStatus Pending = new OrderStatus(1, "Pending");
    public static readonly OrderStatus Shipped = new OrderStatus(2, "Shipped");
    public static readonly OrderStatus Delivered = new OrderStatus(3, "Delivered");

    protected OrderStatus(int id, string name) : base(id, name)
    {
    }
}
```
In this example, `OrderStatus` is an enumeration that represents the different statuses of an order. The `Enumeration` base class provides methods for comparing instances, retrieving all values, and other utility operations.

