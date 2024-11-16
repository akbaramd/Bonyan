# Domain Services in Bonyan.Layer.Domain

This guide explains the concept of domain services in **Bonyan.Layer.Domain** and how to use them effectively to manage business logic that doesn't naturally fit within a specific entity or value object.

## Overview
Domain services are a core part of domain-driven design (DDD) that encapsulate domain logic which doesn't belong to any particular entity. In **Bonyan.Layer.Domain**, domain services provide reusable business operations that can span multiple entities and support complex business workflows.

The **BonDomainService** base class provides a foundation for creating domain services using property injection through **LazyServiceProvider**, which facilitates dependency injection in a convenient and lazy manner, especially when using Autofac as the DI container.

## BonDomainService Base Class
The **BonDomainService** base class provides common dependencies that are needed across domain services, such as logging and domain event dispatching. It also makes use of property injection to provide the required services via **LazyServiceProvider**.

Here is the base definition of the **BonDomainService** class:

```csharp
using Bonyan.DependencyInjection;
using Bonyan.Layer.Domain.DomainEvent.Abstractions;
using Bonyan.Layer.Domain.DomainService;
using Bonyan.MultiTenant;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Bonyan.Layer.Domain.Services;

public abstract class BonDomainService : BonLayServiceProviderConfigurator, IBonDomainService
{
    protected ILoggerFactory LoggerFactory => LazyServiceProvider.LazyGetRequiredService<ILoggerFactory>();
    protected IBonCurrentTenant BonCurrentTenant => LazyServiceProvider.LazyGetRequiredService<IBonCurrentTenant>();
    protected IBonDomainEventDispatcher? DomainEventDispatcher => LazyServiceProvider.LazyGetService<IBonDomainEventDispatcher>();
    protected ILogger Logger => LazyServiceProvider.LazyGetService<ILogger>(provider => LoggerFactory?.CreateLogger(GetType().FullName!) ?? NullLogger.Instance);
}
```

### Key Features
- **LazyServiceProvider Mechanism**: The **LazyServiceProvider** is used to inject dependencies only when they are accessed. This reduces startup time and ensures dependencies are only resolved when needed.
- **Logger**: Provides logging capabilities using **ILogger**.
- **Domain Event Dispatcher**: Handles domain events by dispatching them to interested subscribers.
- **Multi-Tenant Support**: Access to **IBonCurrentTenant** for handling multi-tenancy scenarios.

## Creating a Domain Service
To create a domain service, inherit from **BonDomainService** and add your business logic. You can access common services like **Logger** or **DomainEventDispatcher** through the protected properties provided by the base class.

Below is an example of creating a domain service to handle customer operations:

```csharp
using Bonyan.Layer.Domain.DomainService;

namespace Bonyan.Layer.Domain.Services;

public class CustomerDomainService : BonDomainService
{
    public BonDomainResult CreateCustomer(string name, string email)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email))
        {
            Logger.LogWarning("Invalid customer data: Name or Email is missing.");
            return BonDomainResult.Failure("Customer name and email are required.");
        }

        // Simulate creating a customer
        Logger.LogInformation("Creating a new customer: {Name}, {Email}", name, email);
        return BonDomainResult.Success();
    }
}
```

### Explanation
- **Logger Usage**: The `Logger` property is used to log important information about the process, such as validation issues or successful operations.
- **Result Handling**: **BonDomainResult** is used to indicate the success or failure of the operation, providing a consistent way to handle domain operation outcomes.

## Example Usage
You would typically use domain services from an application service layer. Here’s an example of how to use the **CustomerDomainService** from an application service:

```csharp
public class CustomerAppService
{
    private readonly CustomerDomainService _customerDomainService;

    public CustomerAppService(CustomerDomainService customerDomainService)
    {
        _customerDomainService = customerDomainService;
    }

    public void RegisterCustomer(string name, string email)
    {
        var result = _customerDomainService.CreateCustomer(name, email);
        result.EnsureSuccess();
        Console.WriteLine("Customer registered successfully.");
    }
}
```

### Explanation
- **Dependency Injection**: The **CustomerAppService** depends on **CustomerDomainService**, which is injected through the constructor.
- **Result Handling**: The **EnsureSuccess** method is called to ensure that the operation was successful, throwing an exception if it wasn't.

## Summary
- **BonDomainService**: Base class for domain services, providing property injection via **LazyServiceProvider**.
- **Logging and Domain Events**: Provides access to logging, multi-tenancy, and domain event dispatching, simplifying the implementation of business logic.
- **BonDomainResult**: Used for indicating the success or failure of operations, allowing for consistent error handling.
- **LazyServiceProvider**: Dependencies are injected lazily, improving performance and reducing unnecessary initializations.

Domain services in **Bonyan.Layer.Domain** help encapsulate complex business logic that doesn’t belong in a spe