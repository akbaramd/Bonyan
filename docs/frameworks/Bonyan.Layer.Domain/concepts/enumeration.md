# Enumerations in Bonyan.Layer.Domain

This guide explains the concept of enumerations in **Bonyan.Layer.Domain** and how to use them effectively in your domain-driven design (DDD) projects. Enumerations provide a strongly-typed way to represent a fixed set of related values, similar to enums in C#, but with additional features.

## Overview
The **BonEnumeration** class in **Bonyan.Layer.Domain** represents an advanced version of C# enums that allows you to define strongly-typed values with additional functionality. Enumerations are particularly useful when you need to enrich enum-like constructs with behavior or more complex properties.

The **BonEnumeration** class inherits from **BonValueObject**, allowing enumerations to enjoy value-based equality, immutability, and other value object characteristics.

### Base Class
The **Bonyan.Layer.Domain** library provides a base class for implementing strongly-typed enumerations:

- **BonEnumeration**: Represents a base class for enumerations that are more flexible and feature-rich than traditional enums.

## Usage
The `BonEnumeration` class allows you to define enumerations with both an `Id` and a `Name`. Below is an example of how to define and work with enumerations using **BonEnumeration**.

Here is an example of defining an enumeration:

```csharp
using Bonyan.Layer.Domain.Enumerations;

public class OrderStatus : BonEnumeration
{
    public static readonly OrderStatus Pending = new OrderStatus(1, "Pending");
    public static readonly OrderStatus Shipped = new OrderStatus(2, "Shipped");
    public static readonly OrderStatus Delivered = new OrderStatus(3, "Delivered");

    private OrderStatus(int id, string name) : base(id, name)
    {
    }
}
```

In the above example, `OrderStatus` is an enumeration with predefined values such as `Pending`, `Shipped`, and `Delivered`. Each value has a unique `Id` and a descriptive `Name`. This approach allows you to add more behavior or utility methods to the enumeration as needed.

### Using Enumerations in Domain Model
Enumerations are often used to represent specific states or types within entities, making your domain model more expressive and strongly typed. Here is an example of how to use an enumeration within an entity:

```csharp
using Bonyan.Layer.Domain.Entity;

public class Order : BonEntity<int>
{
    public OrderStatus Status { get; private set; }

    public Order(int id, OrderStatus status)
    {
        Id = id;
        Status = status;
    }

    public void ShipOrder()
    {
        if (Status != OrderStatus.Pending)
        {
            throw new InvalidOperationException("Only pending orders can be shipped.");
        }
        Status = OrderStatus.Shipped;
    }
}
```

In the above example, `Order` is an entity that uses `OrderStatus` to represent the current status of an order. The use of `BonEnumeration` makes the status more descriptive and provides built-in safety when changing states.

### Retrieving Instances
The `BonEnumeration` class provides several utility methods to retrieve instances of the enumeration.

#### Get All Instances
You can retrieve all instances of an enumeration using the `GetAll` method:

```csharp
var allStatuses = BonEnumeration.GetAll<OrderStatus>();
foreach (var status in allStatuses)
{
    Console.WriteLine(status.Name);
}
```

This method is useful when you need to iterate through all possible values of an enumeration, for example, when populating dropdown menus in a UI.

#### Get by Id or Name
You can retrieve an enumeration instance by its `Id` or `Name` using the `FromId` or `FromName` methods:

```csharp
var statusById = OrderStatus.FromId<OrderStatus>(1); // Returns OrderStatus.Pending
var statusByName = OrderStatus.FromName<OrderStatus>("Shipped"); // Returns OrderStatus.Shipped
```

These methods allow you to safely retrieve an enumeration instance based on its identifier or name, reducing the risk of errors.

#### Try Parse by Id or Name
If you're not sure whether an `Id` or `Name` exists in the enumeration, you can use the `TryParse` methods to safely attempt to retrieve an instance:

```csharp
if (OrderStatus.TryParse(2, out var shippedStatus))
{
    Console.WriteLine($"Status found: {shippedStatus.Name}");
}

if (OrderStatus.TryParse("Delivered", out var deliveredStatus))
{
    Console.WriteLine($"Status found: {deliveredStatus.Name}");
}
```

#### Get Names and Ids
You can also retrieve the names and IDs of all instances of the enumeration:

```csharp
var allNames = BonEnumeration.GetNames<OrderStatus>();
foreach (var name in allNames)
{
    Console.WriteLine(name);
}

var allIds = BonEnumeration.GetIds<OrderStatus>();
foreach (var id in allIds)
{
    Console.WriteLine(id);
}
```

This functionality is useful when you need a list of available enumeration names or IDs for further processing.

### Comparison
`BonEnumeration` implements `IComparable`, allowing you to compare enumeration instances by their `Id`. You can use standard comparison operators to determine their relative order:

```csharp
if (OrderStatus.Pending < OrderStatus.Shipped)
{
    Console.WriteLine("Pending comes before Shipped.");
}
```

This feature is particularly useful when you need to order or sort enumeration values.


## Summary
- **BonEnumeration**: Represents a base class for creating strongly-typed enumerations that provide additional functionality over standard C# enums.
- **Defining Enumerations**: Define your enumeration by inheriting from `BonEnumeration` and creating static instances for each value.
- **Retrieving Instances**: Use methods like `GetAll`, `FromId`, and `FromName` to retrieve enumeration instances safely.
- **Try Parsing**: Use `TryParse` to safely attempt retrieval of enumeration values.
- **Get Names and Ids**: Retrieve all enumeration names and IDs for further processing.
- **Comparison and Ordering**: `BonEnumeration` supports comparison and ordering, making it suitable for scenarios requiring sorted values.
- **Entity Framework Integration**: Use the `GetValueComparer` method for easy integration with EF Core.

Using **BonEnumeration** in **Bonyan.Layer.Domain** provides you with a powerful way to create rich, strongly-typed enumerations that are more descriptive and flexible than traditional enums. This makes your domain model more robust, expressive, and easier to maintain.

