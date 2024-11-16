# Entities

This guide will explain how to use entities in **Bonyan.Layer.Domain**, providing examples for both strongly-typed key entities and keyless entities (where keys must be defined explicitly).

## Overview
Entities are the core of your domain model, representing key concepts and objects in your business logic. In **Bonyan.Layer.Domain**, entities are built to ensure consistency and maintainability, providing a solid foundation for representing your data.

### Entity Base Classes
The **Bonyan.Layer.Domain** library provides base classes for entities:

- **BonEntity**: A base class for all entities in the domain, providing a method to retrieve entity keys.
- **BonEntity<TKey>**: A base class for entities with a strongly-typed key (`TKey`).

These classes help ensure that all entities conform to a consistent structure for identifying and managing keys.

### Entity Abstractions
The entity base classes implement the following interfaces, which are defined in **Bonyan.Layer.Domain.Abstractions**:

```csharp
namespace Bonyan.Layer.Domain.Entity;

public interface IBonEntity
{
    object[] GetKeys();
}

public interface IBonEntity<TKey> : IBonEntity
{
    TKey Id { get; set; }
}
```

These interfaces provide a standardized contract for all entities, ensuring that the methods to retrieve entity keys are consistently implemented across different entity types.

### BonEntity: Base Class for Keyless Entities
The `BonEntity` class can be used as a base class for entities that do not have a strongly-typed key, but where you still need to define the entity's keys explicitly.

Here is an example of defining a keyless entity:

```csharp
using Bonyan.Layer.Domain.Entity;

public class Product : BonEntity
{
    public string Name { get; set; }
    public decimal Price { get; set; }

    // Override the GetKeys method to define the unique key(s) for this entity
    public override object[] GetKeys()
    {
        return new object[] { Name }; // Using Name as the unique key
    }
}
```

In the above example, `Product` inherits from `BonEntity` and overrides the `GetKeys` method to provide a unique key (`Name` in this case). This approach allows you to define custom identifiers for entities that do not use a standard key type.

### BonEntity<TKey>: Base Class for Strongly-Typed Key Entities
The `BonEntity<TKey>` class is used when the entity has a strongly-typed key. This provides a more type-safe and consistent way to work with keys in your domain.

Here is an example of using `BonEntity<TKey>`:

```csharp
using Bonyan.Layer.Domain.Entity;

public class Order : BonEntity<int>
{
    public int OrderNumber { get; set; }
    public DateTime OrderDate { get; set; }

    // The Id property is inherited from BonEntity<TKey> and serves as the unique key
}
```

In the above example, `Order` inherits from `BonEntity<int>`, which means that it has an `Id` property of type `int` that serves as the unique identifier for the entity. This approach is ideal for entities where a simple, strongly-typed key is sufficient.


## Summary
- **BonEntity**: Use this for keyless entities where you need to manually define the keys by overriding `GetKeys`.
- **BonEntity<TKey>**: Use this for entities with a strongly-typed key, where the `Id` property is automatically provided and managed.
- **Entity Abstractions**: Use `IBonEntity` and `IBonEntity<TKey>` interfaces from **Bonyan.Layer.Domain.Abstractions** to provide a standardized structure for your entities.

By using the base classes and abstractions provided by **Bonyan.Layer.Domain**, you ensure that your entities follow consistent practices for defining and managing keys, making your domain model more maintainable and robust.

