# Bonyan Modular Application Framework Documentation

Welcome to the **Bonyan Modular Application Framework** documentation. Bonyan offers a sophisticated framework designed
specifically for .NET Core developers aiming to construct highly scalable, maintainable, and modular applications.
Leveraging the concept of **modules** and **dependency management**, Bonyan facilitates the decomposition of complex
applications into self-contained, interrelated components, promoting a refined architectural paradigm.

This guide provides an in-depth exploration of the **purpose** of Bonyan, detailed installation steps, comprehensive
usage examples, the creation of modules, and the lifecycle management of modules, ensuring you can effectively utilize
Bonyan's full capabilities to create modular, extensible, and dynamic .NET Core applications.

## 1. Introduction

**Bonyan** provides an advanced modular framework intended to ensure that your .NET Core applications are **modular** by
design. In Bonyan, a **module** is a fundamental abstraction—a self-contained unit encapsulating a specific set of
functionalities, thereby making the architecture inherently scalable, maintainable, and reusable.

- **Modular Structure**: Your application is segmented into discrete modules, each responsible for a distinct subsystem,
  such as security, data persistence, or API management.
- **Dependency Management**: Modules can declare inter-module dependencies, allowing for a clean and predictable
  initialization order.
- **Lifecycle Management**: Bonyan provides a comprehensive set of lifecycle hooks that permit configuration of modules
  at different stages—from startup to runtime—granting developers fine-grained control over module behavior.

The principal advantage of Bonyan lies in its capacity to decouple system components, thereby enabling developers to
manage sophisticated systems with heightened precision, scalability, and maintainability.

## 2. Installation

To integrate Bonyan into your .NET Core project, execute the following command:

```bash
dotnet add package Bonyan.AspNetCore
```

This command adds the Bonyan library to your project, unlocking its powerful modular capabilities and tools.

## 3. Getting Started

After installing Bonyan, initiate an application using `BonyanApplication.CreateApplicationBuilder`. This command
establishes an **application builder** that configures modules through your specified **main application module**:

```csharp
var builder = BonyanApplication.CreateApplicationBuilder<YourMainModule>(args);

var app = builder.Build();

context.Run();
```

In this example, `YourMainModule` acts as the **entry point**, defining the core structure and dependencies for your
entire application. Modules can be organized hierarchically, facilitating seamless dependency management, lifecycle
orchestration, and configuration.

## 4. Creating a Modular Application

Bonyan's modular approach ensures that each **module** serves as an isolated unit responsible for specific services,
configurations, or middleware. This architectural style empowers developers to compartmentalize application components,
allowing independent management of each segment of the system.

### Example: Creating a Basic Module

```csharp
using Bonyan.Modularity;
using Bonyan.Modularity.Attributes;

namespace BonyanTemplate.Api
{
    [DependOn(typeof(SecurityModule), typeof(PersistenceModule))]
    public class YourMainModule : Module
    {
        public override Task OnPreConfigureAsync(ModularityContext context)
        {
            // Pre-configure services
            context.Services.AddLogging();
            return base.OnPreConfigureAsync(context);
        }

        public override Task OnConfigureAsync(ModularityContext context)
        {
            // Register application-specific services
            return base.OnConfigureAsync(context);
        }
    }
}
```

- **Dependency Declaration**: Utilizing the `[DependOn]` attribute, `YourMainModule` ensures that `SecurityModule`
  and `PersistenceModule` are loaded prior to its own configuration, thereby establishing an explicit hierarchy of
  dependencies.
- **Lifecycle Control**: The module overrides lifecycle events such as `OnPreConfigureAsync` and `OnConfigureAsync`,
  providing precise control over how and when services are initialized.

## 5. Deep Dive into Modules

In Bonyan, a **Module** represents a self-contained segment of the application. Modules can range in complexity—from
simple components managing basic tasks like logging and configuration to sophisticated modules encompassing entire
subsystems, such as security or data persistence.

### Types of Modules

- **`Module`**: The foundational abstraction for all Bonyan modules, offering standard lifecycle hooks for
  general-purpose application configuration.
- **`WebModule`**: A specialized extension of `Module`, designed for web applications, including additional lifecycle
  events that are essential for handling HTTP requests, middleware, and other web-centric processes.

### Dependency Management

Modules declare dependencies through the `[DependOn]` attribute. Bonyan utilizes this information to establish a *
*loading order**, ensuring that all dependencies are satisfied before a module proceeds with its configuration.

```csharp
[DependOn(typeof(SecurityModule), typeof(PersistenceModule))]
public class ApplicationModule : Module { }
```

Dependencies are loaded **early**, guaranteeing that essential services are available for modules that require their
functionality.

## 6. Web Modules vs. Modules

Bonyan introduces two primary types of modules: **Module** and **WebModule**.

### **WebModule**: Specialized for Web Applications

- `WebModule` extends `Module` and is explicitly designed for **web services**.
- It introduces additional lifecycle events, such as:
    - **OnPreApplicationAsync**: Ideal for configuring middleware or other services that must be set before application
      startup.
    - **OnApplicationAsync**: Used for registering critical services like health checks, API endpoints, and other
      runtime services.
    - **OnPostApplicationAsync**: Execute finalization tasks after the main application starts running.

### When to Use `WebModule` vs. `Module`

- Utilize **`Module`** for core application services, configurations, or components that are agnostic of a web context.
- Opt for **`WebModule`** when working with components that require web-specific behavior, such as HTTP APIs,
  middleware, or any startup tasks that are inherently linked to a web server environment.

### Example

```csharp
public class MyWebModule : WebModule
{
    public override Task OnPreApplicationAsync(ModularityApplicationContext context)
    {
        context.Services.Configure<JwtSigningOptions>(options =>
        {
            options.SigningKey = "super-secure-signing-key";
        });
        return Task.CompletedTask;
    }
}
```

In this example, `OnPreApplicationAsync` is overridden to configure JWT settings before the application starts,
demonstrating the web-specific capabilities of `WebModule`.

## 7. Module Lifecycle Events

Bonyan provides an extensive set of lifecycle events that allow developers to control the **configuration** and *
*initialization** of modules with precision.

### General Lifecycle Events (`Module`):

- **OnPreConfigureAsync**: Executed before the main configuration phase. Ideal for adding logging services or performing
  early configuration tasks.

- **OnConfigureAsync**: The primary configuration stage where core services and options are registered.

- **OnPostConfigureAsync**: Run after the main configuration, suitable for finalizing configuration settings.

- **OnPreInitializeAsync**: Executed before the initialization phase begins.

- **OnInitializeAsync**: Handles tasks during the module initialization phase.

- **OnPostInitializeAsync**: Runs once initialization is complete.

### Additional Web Lifecycle Events (`WebModule`):

- **OnPreApplicationAsync**: Critical for configuring middleware or other pre-startup services.
- **OnApplicationAsync**: Registers application-specific services, such as API endpoints, during startup.
- **OnPostApplicationAsync**: Finalizes configurations or handles any post-startup tasks for web applications.

## 8. Advanced Module Example

Below is an advanced example demonstrating how to create a modular application using Bonyan, highlighting the dependency
and lifecycle management aspects of the framework.

```csharp
using Bonyan.Modularity;
using Bonyan.Modularity.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace BonyanAdvancedExample
{
    [DependOn(typeof(DataModule), typeof(SecurityModule))]
    public class MainApplicationModule : WebModule
    {
        public override Task OnPreConfigureAsync(ModularityContext context)
        {
            // Configure services before the main configuration
            context.Services.AddLogging();
            return base.OnPreConfigureAsync(context);
        }

        public override Task OnConfigureAsync(ModularityContext context)
        {
            // Register application-specific services or configure options
            context.Services.AddHttpClient();
            return base.OnConfigureAsync(context);
        }

        public override Task OnPreApplicationAsync(ModularityApplicationContext context)
        {
            // Middleware configuration before application startup
            context.Services.Configure<JwtSigningOptions>(options =>
            {
                options.SigningKey = "secure-signing-key";
            });
            return Task.CompletedTask;
        }
    }

    public class DataModule : Module
    {
        public override Task OnConfigureAsync(ModularityContext context)
        {
            // Register data-related services, such as a database context
            context.Services.AddDbContext<MyDbContext>();
            return base.OnConfigureAsync(context);
        }
    }

    public class SecurityModule : Module
    {
        public override Task OnConfigureAsync(ModularityContext context)
        {
            // Configure security-related services, such as authentication
            context.Services.AddAuthentication();
            return base.OnConfigureAsync(context);
        }
    }
}
```

### Summary

- **Dependencies**: The `MainApplicationModule` depends on `DataModule` and `SecurityModule`, ensuring that essential
  data and security configurations are completed before the main module is initialized.
- **Lifecycle Events**: The modules utilize lifecycle hooks to set up services like logging, HTTP clients, JWT signing,
  and database contexts, showcasing how different stages can be used to properly initialize the application.

## 9. Conclusion

The **Bonyan Modular Application Framework** empowers developers to architect modular, extensible, and maintainable .NET
Core applications. Structuring applications using **modules** with well-defined **dependencies** and carefully
orchestrated **lifecycle events** ensures a scalable, maintainable, and clean architecture. The distinction
between `Module` and `WebModule` allows for targeted optimizations—whether your focus is on core application services or
on web-specific middleware and configurations.

Leverage the potential of modularity through Bonyan to create robust, scalable applications capable of evolving
alongside the ever-changing demands of modern software development. Dive deeper into Bonyan's capabilities, and let
modularity form the foundation of your architectural strategy.

