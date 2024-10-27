# Domain-Driven Design (DDD) Module Guide

The **Bonyan.DomainDrivenDesign.Domain** module is intended to facilitate the implementation of Domain-Driven Design (DDD) principles within .NET Core applications. This module provides the foundational abstractions—**Entities**, **Aggregate Roots**, **Value Objects**, and **Enumerations**—essential for modeling sophisticated software domains effectively, thereby enhancing both maintainability and domain clarity.

## Table of Contents
- [Introduction](#introduction)
- [Installation](#installation)
- [Core DDD Concepts](#core-ddd-concepts)
    - [Entity](#entity)
    - [Aggregate Root](#aggregate-root)
    - [Value Object](#value-object)
    - [Enumeration](#enumeration)
- [Usage Examples](#usage-examples)
    - [Defining an Entity](#defining-an-entity)
    - [Defining an Aggregate Root](#defining-an-aggregate-root)
- [Summary](#summary)

## Introduction

Domain-Driven Design represents an advanced methodology in software development that emphasizes deep collaboration between domain experts and software developers to ensure that the domain model captures the nuances of the business domain with precision. The **Bonyan.DomainDrivenDesign.Domain** module equips .NET Core developers with abstractions that facilitate the seamless adoption of DDD principles. By offering critical components out-of-the-box, this module simplifies the construction of complex and rich domain models, thereby allowing developers to focus on capturing intricate business logic accurately.

 Additionally, any module that requires access to these foundational DDD constructs must also declare a dependency on `BonyanDomainDrivenDesignDomainModule` to ensure seamless integration of domain modeling capabilities. This ensures that all the necessary tools for DDD are readily available for use.


## Add Domain Module Dependency

To use the **Bonyan.DomainDrivenDesign.Domain** module, your main module must declare a dependency on `BonyanDomainDrivenDesignDomainModule`. This ensures that all necessary DDD services and configurations are available for modeling the domain effectively.

Here is how to declare the dependency in your module:

```csharp
[DependOn(typeof(BonyanDomainDrivenDesignDomainModule))]
public class MyMainModule : Module
{
    public override Task OnConfigureAsync(ModularityContext context)
    {
        // Example setup code for domain-driven components
        return base.OnConfigureAsync(context);
    }
}
```

In this example, the `MyMainModule` depends on `BonyanDomainDrivenDesignDomainModule`, ensuring that domain modeling constructs such as entities, aggregates, and value objects are available.


```bash
dotnet add package Bonyan.DomainDrivenDesign.Domain
```

This command integrates the necessary libraries and abstractions needed for implementing DDD principles in your .NET Core project.

## Core DDD Concepts

Once the module is integrated, you can utilize the foundational classes such as `Entity`, `AggregateRoot`, `ValueObject`, and others to construct a rich and effective domain model.

### Entity

An **Entity** is an object defined by its unique identity, which persists throughout its lifecycle. Entities serve as the primary building blocks of a domain model, encapsulating the core business objects and their associated lifecycle. Within **Bonyan.DomainDrivenDesign.Domain**, the `Entity` class functions as the base class for defining entities in your domain.

```csharp
public class Customer : Entity<Guid>
{
    public string Name { get; private set; }
    public string Email { get; private set; }

    public Customer(Guid id, string name, string email) : base(id)
    {
        Name = name;
        Email = email;
    }
}
```

In the above example, `Customer` is an entity characterized by properties `Name` and `Email`. By inheriting from the `Entity` base class, each `Customer` instance is ensured to possess a unique identifier (`Guid`).

### Aggregate Root

An **Aggregate Root** is an entity that serves as the entry point for a cluster of related objects (an aggregate) and governs their lifecycle. Aggregates help enforce business rules and consistency boundaries. In **Bonyan.DomainDrivenDesign.Domain**, the `AggregateRoot` class facilitates the definition of these aggregate roots.

```csharp
public class Order : AggregateRoot<Guid>
{
    public DateTime OrderDate { get; private set; }
    public List<OrderItem> Items { get; private set; }

    public Order(Guid id, DateTime orderDate) : base(id)
    {
        OrderDate = orderDate;
        Items = new List<OrderItem>();
    }

    public void AddItem(OrderItem item)
    {
        Items.Add(item);
    }
}
```

In this example, `Order` functions as an aggregate root that manages a collection of `OrderItem` objects, ensuring the consistency and integrity of its internal state.

### Value Object

A **Value Object** is a concept that lacks an inherent identity, meaning its equality is determined by its attribute values. Value objects are useful for representing concepts such as money, measurements, or addresses, where uniqueness is not as important as the value they hold.

```csharp
public class Address : ValueObject
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

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return PostalCode;
    }
}
```

The `Address` class represents a value object, consisting of properties `Street`, `City`, and `PostalCode`. The equality of `Address` instances is determined by these property values through the `GetEqualityComponents` method.

### Enumeration

An **Enumeration** is a specialized object that represents a set of predefined values, offering an alternative to primitive types or traditional enumerations. It enhances domain modeling by improving type safety and reducing ambiguity in the codebase.

```csharp
public class OrderStatus : Enumeration
{
    public static readonly OrderStatus Pending = new OrderStatus(1, "Pending");
    public static readonly OrderStatus Shipped = new OrderStatus(2, "Shipped");
    public static readonly OrderStatus Delivered = new OrderStatus(3, "Delivered");

    public OrderStatus(int id, string name) : base(id, name) { }
}
```

In this example, `OrderStatus` defines possible states for an order, such as `Pending`, `Shipped`, and `Delivered`. This helps to maintain consistency in the domain model, reducing the reliance on loosely typed values like strings or integers.

## Usage Examples

### Defining an Entity

To define an entity, extend the `Entity<T>` class provided by the module. The `T` type parameter represents the type of the unique identifier for the entity.

```csharp
public class Product : Entity<int>
{
    public string ProductName { get; private set; }
    public decimal Price { get; private set; }

    public Product(int id, string productName, decimal price) : base(id)
    {
        ProductName = productName;
        Price = price;
    }
}
```

In this example, `Product` is a simple entity characterized by `ProductName` and `Price` properties.

### Defining an Aggregate Root

To define an aggregate root, extend the `AggregateRoot<T>` class. This ensures that the aggregate root manages its children entities effectively, enforcing domain rules and consistency.

```csharp
public class ShoppingCart : AggregateRoot<int>
{
    public List<CartItem> CartItems { get; private set; }

    public ShoppingCart(int id) : base(id)
    {
        CartItems = new List<CartItem>();
    }

    public void AddItem(CartItem item)
    {
        CartItems.Add(item);
    }
}
```

The `ShoppingCart` class serves as an aggregate root managing a list of `CartItem` objects, and it provides operations that ensure the integrity of the aggregate's state.

## Summary

The **Bonyan.DomainDrivenDesign.Domain** module provides fundamental constructs for incorporating Domain-Driven Design principles within .NET Core. By using entities, aggregate roots, value objects, and enumerations, developers can construct expressive and comprehensive domain models that encapsulate complex business logic. Leveraging these abstractions ensures that applications remain maintainable, scalable, and thoroughly aligned with the intricate requirements of the business domain they represent.

