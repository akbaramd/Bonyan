# Bonyan.Layer.Application Module Guide

The **Bonyan.Layer.Application** module is designed to streamline the development of application services in .NET Core projects. This module provides key abstractions such as **Application Services**, **Application Exceptions**, and foundational service interfaces like **IApplicationService** and **ApplicationService**. These components simplify the creation of an application layer that interacts between the domain and infrastructure layers, offering a clean separation of business logic and application logic.

## Table of Contents
- [Introduction](#introduction)
- [Installation](#installation)
- [Core Application Layer Concepts](#core-application-layer-concepts)
  - [Application Service](#application-service)
  - [Exception Handling](#exception-handling)
- [Usage Examples](#usage-examples)
  - [Creating an Application Service](#creating-an-application-service)
  - [Handling Application Exceptions](#handling-application-exceptions)
- [Summary](#summary)

## Introduction

The **Bonyan.Layer.Application** module is part of the layered architecture approach in Domain-Driven Design, specifically focusing on the **Application Layer**. The Application Layer is responsible for coordinating domain operations, handling user requests, and integrating with other parts of the system, such as infrastructure services. This layer is vital in keeping the **domain model** pure from external dependencies and in orchestrating **use cases**.

The **Bonyan.Layer.Application** module includes key components that help developers build this layer efficiently, including **IApplicationService**, **ApplicationService**, and **ApplicationException**. By utilizing these abstractions, developers can focus on implementing business processes without worrying about low-level concerns.

To use this module, any component requiring access to the Application Layer should declare a dependency on `BonyanLayerApplicationModule`.

## Add Application Module Dependency

To use the **Bonyan.Layer.Application** module, your main module must declare a dependency on `BonyanLayerApplicationModule`. This ensures that all the necessary services and configurations are available for managing application-level services.

Here is how to declare the dependency in your module:

```csharp
[DependOn(typeof(BonyanLayerApplicationModule))]
public class MyMainModule : Module
{
    public override Task OnConfigureAsync(ModularityContext context)
    {
        // Example setup code for application-level components
        return base.OnConfigureAsync(context);
    }
}
```

In this example, the `MyMainModule` depends on `BonyanLayerApplicationModule`, making all relevant application services available in the project.

To install the necessary library, use the following command:

```bash
dotnet add package Bonyan.Layer.Application
```

This command integrates the necessary libraries needed for implementing application-level services in your .NET Core project.

## Core Application Layer Concepts

### Application Service

An **Application Service** is a service that encapsulates and orchestrates multiple domain operations. It acts as a middle layer between user interactions and the underlying domain logic. The **Bonyan.Layer.Application** module provides `ApplicationService` as a base class for defining application services in a consistent and reusable way.

The `IApplicationService` interface is also provided to define the base contract that all application services should follow. This helps in creating a standardized approach to application logic.

```csharp
public class CustomerAppService : ApplicationService, IApplicationService
{
    private readonly IRepository<Customer, Guid> _customerRepository;

    public CustomerAppService(IRepository<Customer, Guid> customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<CustomerDto> GetCustomerAsync(Guid id)
    {
        var customer = await _customerRepository.GetAsync(id);
        return new CustomerDto
        {
            Id = customer.Id,
            Name = customer.Name,
            Email = customer.Email
        };
    }
}
```

In the above example, `CustomerAppService` inherits from `ApplicationService` and implements `IApplicationService`, ensuring consistency and making use of available functionality for managing customer data.

### Exception Handling

The **Application Layer** should handle any exceptions that occur within the boundaries of application services, providing a clear and user-friendly experience. The **Bonyan.Layer.Application** module provides an `ApplicationException` class to handle application-specific errors.

By leveraging `ApplicationException`, developers can ensure that exceptions thrown within the Application Layer are descriptive and consistent, which ultimately leads to better error management across the application.

```csharp
public class CustomerNotFoundException : ApplicationException
{
    public CustomerNotFoundException(Guid customerId)
        : base($"Customer with ID {customerId} was not found.")
    {
    }
}
```

In the example above, `CustomerNotFoundException` extends `ApplicationException`, providing a meaningful message when a customer cannot be found. This makes the error messages more informative and helps to ensure consistent error handling.

## Usage Examples

### Creating an Application Service

To create an application service, you extend the `ApplicationService` base class and optionally implement `IApplicationService`. Application services should coordinate user requests and domain logic, without containing any business logic themselves.

```csharp
public class OrderAppService : ApplicationService, IApplicationService
{
    private readonly IRepository<Order, Guid> _orderRepository;

    public OrderAppService(IRepository<Order, Guid> orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderDto> PlaceOrderAsync(OrderDto orderDto)
    {
        var order = new Order(orderDto.Id, orderDto.OrderDate);
        foreach (var itemDto in orderDto.Items)
        {
            order.AddItem(new OrderItem(itemDto.ProductId, itemDto.Quantity));
        }
        await _orderRepository.InsertAsync(order);
        return orderDto;
    }
}
```

In this example, the `OrderAppService` interacts with the domain layer to place orders. The actual business rules for adding items and validating them reside within the domain entities (`Order` and `OrderItem`). The application service simply coordinates these actions.

### Handling Application Exceptions

Application exceptions should be used to provide meaningful error feedback in case of any issues during the execution of application logic.

```csharp
public class OrderAppService : ApplicationService, IApplicationService
{
    private readonly IRepository<Order, Guid> _orderRepository;

    public OrderAppService(IRepository<Order, Guid> orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderDto> GetOrderAsync(Guid id)
    {
        var order = await _orderRepository.FindAsync(id);
        if (order == null)
        {
            throw new OrderNotFoundException(id);
        }

        return new OrderDto
        {
            Id = order.Id,
            OrderDate = order.OrderDate,
            Items = order.Items.Select(item => new OrderItemDto
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity
            }).ToList()
        };
    }
}

public class OrderNotFoundException : ApplicationException
{
    public OrderNotFoundException(Guid orderId) : base($"Order with ID {orderId} was not found.")
    {
    }
}
```

In this example, `OrderNotFoundException` is used to handle cases where the requested order does not exist in the database. This ensures that a meaningful and consistent message is returned to the client.

## Summary

The **Bonyan.Layer.Application** module provides foundational components for constructing the Application Layer in .NET Core. With abstractions like **Application Services**, **IApplicationService**, and **ApplicationException**, developers can maintain a clean separation of concerns and streamline the development of application services. This approach enhances maintainability, promotes standardization, and ensures that application logic remains distinct from domain and infrastructure concerns.

