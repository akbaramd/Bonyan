# Bonyan.AutoMapper Module Guide

The **Bonyan.AutoMapper** module is designed to simplify the process of mapping objects in .NET Core applications using AutoMapper. This module provides necessary infrastructure for defining object mapping profiles and integrating validations to ensure data transformation is handled correctly and efficiently across different layers of your application.

## Table of Contents
- [Introduction](#introduction)
- [Installation](#installation)
- [Core Features](#core-features)
    - [Profiles](#profiles)
    - [Validators](#validators)
- [Usage Examples](#usage-examples)
    - [Configuring Profiles](#configuring-profiles)
    - [Setting up Validators](#setting-up-validators)
- [Summary](#summary)

## Introduction

Object mapping is a common practice in software development, used to transform data between different models and layers. The **Bonyan.AutoMapper** module integrates with AutoMapper, a popular library for object mapping in .NET, to facilitate this transformation seamlessly.

This module not only provides an easy way to set up mapping configurations but also ensures that the mapping logic remains consistent across your application. With a standardized approach for managing profiles and validating transformations, the **Bonyan.AutoMapper** module ensures correctness and efficiency in data transfer.

## Add AutoMapper Module Dependency

To use the **Bonyan.AutoMapper** module, your main module must declare a dependency on `BonyanAutoMapperModule`. This ensures that all necessary AutoMapper configurations are available for mapping objects effectively.

Here is how to declare the dependency in your module:

```csharp
[DependOn(typeof(BonyanAutoMapperModule))]
public class MyMainModule : Module
{
    public override Task OnConfigureAsync(ModularityContext context)
    {
        Configure<BonyanAutoMapperOptions>(options =>
        {
            // Setup AutoMapper profiles and validators here
        });
        return base.OnConfigureAsync(context);
    }
}
```

In this example, the `MyMainModule` depends on `BonyanAutoMapperModule`, ensuring that the configuration options for setting up profiles and validators are readily available.

To install the necessary library, use the following command:

```bash
dotnet add package Bonyan.AutoMapper
```

This command integrates the necessary library needed for handling AutoMapper profiles and validators in your .NET Core project.

## Core Features

### Profiles

A **Profile** in AutoMapper defines a mapping configuration between source and destination types. By using profiles, you can encapsulate and organize your mapping logic within different classes, which can then be registered to the AutoMapper engine. The **Bonyan.AutoMapper** module makes it straightforward to define and register profiles for different application components.

```csharp
public class CustomerProfile : Profile
{
    public CustomerProfile()
    {
        CreateMap<Customer, CustomerDto>();
        CreateMap<CustomerDto, Customer>();
    }
}
```

In the above example, `CustomerProfile` defines mapping configurations for converting between `Customer` and `CustomerDto` objects, making it easy to transform data as needed.

### Validators

The **Validators** are useful for validating the mapped objects to ensure data consistency and correctness. By configuring validation in the **Bonyan.AutoMapper** module, you can prevent incorrect data from being mapped between different layers of the application.

```csharp
public class CustomerValidator : AbstractValidator<CustomerDto>
{
    public CustomerValidator()
    {
        RuleFor(customer => customer.Name).NotEmpty().WithMessage("Customer name is required.");
        RuleFor(customer => customer.Email).EmailAddress().WithMessage("A valid email is required.");
    }
}
```

In this example, `CustomerValidator` is used to ensure that the `CustomerDto` object meets certain criteria before or after mapping. This helps to ensure that the data in different layers remains valid and consistent.

## Usage Examples

### Configuring Profiles

To configure a profile, extend the `Profile` class provided by AutoMapper, and then add your specific mapping configurations. Profiles help you to maintain clean and modular mapping code.

```csharp
public class OrderProfile : Profile
{
    public OrderProfile()
    {
        CreateMap<Order, OrderDto>();
        CreateMap<OrderItem, OrderItemDto>();
    }
}
```

The `OrderProfile` above defines mappings for `Order` to `OrderDto` and `OrderItem` to `OrderItemDto`. This allows seamless data transformation between domain entities and their corresponding DTOs.

To register this profile with the **Bonyan.AutoMapper** module, use the `BonyanAutoMapperOptions` during module configuration:

```csharp
public override Task OnConfigureAsync(ModularityContext context)
{
    Configure<BonyanAutoMapperOptions>(options =>
    {
        options.AddProfile<OrderProfile>();
    });
    return base.OnConfigureAsync(context);
}
```

This setup ensures that the `OrderProfile` is registered and available for use within your application's AutoMapper configuration.

### Setting up Validators

Validators help ensure that mapped objects meet certain conditions before being used within the application. You can define a validator for a DTO and register it with the module.

```csharp
public class ProductValidator : AbstractValidator<ProductDto>
{
    public ProductValidator()
    {
        RuleFor(product => product.ProductName).NotEmpty().WithMessage("Product name must be provided.");
        RuleFor(product => product.Price).GreaterThan(0).WithMessage("Price must be greater than zero.");
    }
}
```

To register the validator with the **Bonyan.AutoMapper** module:

```csharp
public override Task OnConfigureAsync(ModularityContext context)
{
    Configure<BonyanAutoMapperOptions>(options =>
    {
        options.AddValidator<ProductValidator>();
    });
    return base.OnConfigureAsync(context);
}
```

This setup will ensure that the `ProductValidator` is used to validate any `ProductDto` object during mapping or data transformation processes.

## Summary

The **Bonyan.AutoMapper** module provides essential tools for integrating AutoMapper into your .NET Core application effectively. With features like profiles and validators, developers can ensure smooth data transformation while maintaining data integrity and consistency. By leveraging profiles, you can manage mapping logic modularly, and by using validators, you ensure that your application's data conforms to expected standards. This approach improves maintainability and reduces the risk of data-related issues across different layers of your application.

