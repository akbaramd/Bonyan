# Introduction to Bonyan.Layer.Domain

Welcome to the **Bonyan.Layer.Domain** documentation! This guide will help you understand what this module is about, how to install it in your project, and how to use it effectively.

## What is Bonyan.Layer.Domain?

The **Bonyan.Layer.Domain** module provides a foundational layer for defining the core business logic of your application. The domain layer is where the key business rules, entities, and processes reside, encapsulating the core knowledge and functionality of the system. It is responsible for representing the real-world concepts, rules, and interactions that are important to your application.

### Domain and Domain Layer Explained

- **Domain**: The domain represents the core problem space or business area your application is designed to address. It encompasses the key concepts and rules that are crucial to the business or purpose of the software.
- **Domain Layer**: The domain layer is a logical layer in your software architecture that contains all the domain entities, value objects, aggregates, and domain services. This layer is agnostic of infrastructure concerns and is focused solely on implementing the business rules and logic.

The **Bonyan.Layer.Domain** module helps you create a clean separation of business logic from other layers of your application, enabling a more maintainable and testable architecture.

## Prerequisites

- **.NET Core**: Version {{netCoreVersion}} or later is recommended.
- **NuGet Package Manager**: To install the library dependencies.

## Installation

To install **Bonyan.Layer.Domain**, use one of the following methods:

### .NET CLI

Run the following command in your terminal:

```bash
dotnet add package Bonyan.Layer.Domain --version {{libraryVersion}}
```

### NuGet Package Manager

1. Open Visual Studio.
2. Navigate to **Tools > NuGet Package Manager > Manage NuGet Packages for Solution**.
3. Search for `Bonyan.Layer.Domain`.
4. Click **Install**.

## Usage

The **Bonyan.Layer.Domain** library can be used in two main ways, depending on your project structure and preferences: the **Modular Approach** and the **Builder Approach**.

### 1. Modular Approach

The modular approach allows you to use the domain layer in a highly organized and loosely coupled manner, making it easy to manage dependencies between different parts of your application.

Start by creating a new class that inherits from `BonModule`.

```csharp
using Bonyan.Layer.Domain;

public class TestModule : BonModule
{
    public TestModule()
    {
        DependOn<BonLayerDomainModule>();
    }
}
```

In this approach, the **Messaging Module** is automatically added when using the **Bonyan.Layer.Domain** module, ensuring that domain events are handled seamlessly. This approach allows for modularity in your application, making it easier to manage multiple layers and dependencies.

### 2. Builder Approach

If your project uses the builder pattern, you can directly configure the domain layer through the service collection.

Use the `AddBonyan` method in the `IServiceCollection` to add the domain layer to your application.

```csharp
builder.Services.AddBonyan(c => {
    // Note: In the builder approach, you must explicitly add the Messaging Module.
    // This is because domain events require messaging features.
    c.AddMessaging();
    c.AddDomainLayer();
});
```

In the builder approach, make sure to add the **Messaging Module** manually to ensure that domain events are properly handled.

## Additional Resources

- [NuGet Package](https://www.nuget.org/packages/Bonyan.Layer.Domain)
- [GitHub Repository](https://github.com/your-repo/Bonyan.Layer.Domain)

