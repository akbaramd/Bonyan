# Value Objects

This guide explains the concept of value objects in **Bonyan.Layer.Domain** and how to use them effectively, including examples to help you understand their role in domain-driven design (DDD).

## Overview
Value objects are a fundamental building block in domain-driven design. Unlike entities, value objects do not have a unique identity. Instead, they represent descriptive aspects of the domain, like amounts of money or measurement units, that are defined by their attributes.

In **Bonyan.Layer.Domain**, value objects are implemented to provide immutability, ensure correct equality operations, and encapsulate the concept of equality based on attribute values rather than identity.

### BonValueObject: Base Class
The **Bonyan.Layer.Domain** library provides a base class for value objects:

- **BonValueObject**: Represents a base class for value objects in a domain-driven design context. It enforces immutability and provides equality operations to ensure that value objects are compared based on their properties rather than identity.

## Usage
The `BonValueObject` class provides the foundation for value objects, offering features such as equality checks, hash code generation, and value comparison. Below is an explanation of how to define and work with value objects.

Here is an example of defining a value object:

```csharp
using Bonyan.Layer.Domain.ValueObjects;

public class Address : BonValueObject
{
    public string Street { get; }
    public string City { get; }
    public string PostalCode { get; }

    public Address(string street, string city, string postalCode)
    {
        Street = street;
        City = city;
        PostalCode = postalCode;
    }

    // Override GetEqualityComponents to define what makes this value object unique
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return PostalCode;
    }
}
```

In this example, `Address` is a value object that inherits from `BonValueObject`. It overrides the `GetEqualityComponents` method to define the properties that determine equality for instances of `Address`. This means two `Address` instances are considered equal if their `Street`, `City`, and `PostalCode` values are all equal.


Value objects are used to represent properties of entities or other value objects, typically as part of the entity's state. Below is an example of how to use a value object within an entity:

```csharp
using Bonyan.Layer.Domain.Entity;

public class Customer : BonEntity<int>
{
    public string Name { get; set; }
    public Address Address { get; set; }

    public Customer(int id, string name, Address address)
    {
        Id = id;
        Name = name;
        Address = address;
    }
}
```

In the above example, `Customer` is an entity that uses `Address` as a value object. Since value objects are immutable, if the customer's address changes, a new `Address` instance should be created and assigned to the `Customer` entity.

#### Equality and Immutability
The `BonValueObject` class provides equality operations, allowing you to compare value objects effectively. Here's how value objects are compared:

- **Equality**: The `Equals` and `GetHashCode` methods are overridden to ensure that two value objects are considered equal if their component values are equal. This allows value objects to be compared by value rather than by reference.
- **Immutability**: Value objects should be immutable. This means that once a value object is created, its state cannot change. This is enforced by only allowing property getters (no setters) and by not exposing any methods that modify the internal state.

#### EF Core Integration
When working with value objects in Entity Framework Core, you may need to use a value comparer to handle collections of value objects correctly. The `BonValueObject` class provides a static method to generate a value comparer:

```csharp
using Microsoft.EntityFrameworkCore.ChangeTracking;

modelBuilder.Entity<Order>()
    .Property(o => o.ShippingAddress)
    .Metadata.SetValueComparer(BonValueObject.GetValueComparer<Address>());
```

This ensures that Entity Framework Core can correctly track changes and compare value objects during persistence.

## Summary
- **Value Objects**: Represent descriptive aspects of the domain without unique identity. They are defined by their attributes and are immutable.
- **BonValueObject**: The base class for value objects in **Bonyan.Layer.Domain**. It provides equality operations and enforces immutability.
- **Defining Value Objects**: Override `GetEqualityComponents` to specify the properties that determine equality.
- **Immutability**: Value objects should be immutable to ensure consistent behavior in the domain model.
- **Using Value Comparer**: When working with value objects in Entity Framework Core, use the provided `GetValueComparer` to handle collections effectively.

Value objects play a crucial role in creating a robust domain model by ensuring consistency, immutability, and value-based equality. Use value objects wherever you need to represent concepts that do not require unique identity but are defined by their attributes.

