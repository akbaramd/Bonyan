# Web [Module](/module.md) Guide

This guide provides an in-depth overview of the **WebModule** within **Bonyan.AspNetCore**. The **WebModule** builds upon the foundation of the base [**[Module](/module.md)**](/module.md) class, introducing specific functionality tailored to web application contexts. We will explore how **WebModule** inherits from the core [**[Module](/module.md)**](/module.md) and the additional lifecycle methods it provides to handle web-specific requirements.

## What is a WebModule?

A **WebModule** in **Bonyan.AspNetCore** extends the functionality of a base [**[Module](/module.md)**](/module.md) by providing specific lifecycle hooks intended to manage the setup and configuration of web applications. Like the core [**[Module](/module.md)**](/module.md), a **WebModule** encapsulates discrete functionality, adheres to modular design principles, and ensures that components remain organized and maintainable within a monolithic architecture.

The **WebModule** inherits all the capabilities of a regular [**[Module](/module.md)**](/module.md), including lifecycle hooks such as **OnPreConfigureAsync**, **OnConfigureAsync**, and **OnPostConfigureAsync**. Additionally, **WebModule** introduces three extra methods specifically designed for managing web application contexts, offering more fine-grained control over web-specific setup and initialization.

## Web [Module](/module.md) Lifecycle Extensions

The **WebModule** extends the core module lifecycle by introducing three additional methods:

### Web Application Lifecycle Methods

1. **OnPreApplicationAsync**: This method is invoked before the web application begins its startup process. It is particularly useful for preliminary setup tasks that must be completed before the application’s middleware pipeline is configured.

   ```csharp
   public virtual Task OnPreApplicationAsync(ApplicationContext context)
   {
       // Preliminary setup for the web application
       return Task.CompletedTask;
   }
   ```

   This lifecycle hook can be used for configuring essential web services, setting up cross-origin resource sharing (CORS) policies, or other tasks that need to be in place before the main web setup.

2. **OnApplicationAsync**: This method is executed during the core startup of the web application. It is used for configuring middleware, setting up routing, or integrating third-party services that need to be part of the application's startup logic.

   ```csharp
   public virtual Task OnApplicationAsync(ApplicationContext context)
   {
       // Core application setup logic here
       return Task.CompletedTask;
   }
   ```

   This hook is crucial for configuring the request pipeline, ensuring that routing, authentication, and other essential components are correctly set up as the application starts.

3. **OnPostApplicationAsync**: This method runs after the application has started successfully and is ready to handle requests. It is useful for post-startup tasks such as logging the application's status, initiating background services, or other tasks that should be completed once the application is fully operational.

   ```csharp
   public virtual Task OnPostApplicationAsync(ApplicationContext context)
   {
       // Final tasks after the application is up and running
       return Task.CompletedTask;
   }
   ```

   This final lifecycle method is ideal for setting up any background jobs, monitoring hooks, or ensuring that all services are running smoothly after the main startup sequence is complete.

## Inheritance from [Module](/module.md)

The **WebModule** class extends the core [**[Module](/module.md)**](/module.md) in **Bonyan.AspNetCore**, which means it inherits all the lifecycle hooks available to a standard module, including:

- **OnPreConfigureAsync**: For preparatory tasks before service configuration.
- **OnConfigureAsync**: For defining core service configurations.
- **OnPostConfigureAsync**: For finalizing configurations after the primary setup.
- **OnPreInitializeAsync**, **OnInitializeAsync**, **OnPostInitializeAsync**: Methods that handle the module's initialization stages.

The additional methods provided by **WebModule** allow developers to control web-specific processes seamlessly, offering a consistent and powerful approach to managing both the generic module setup and the specialized needs of web applications.

## Summary

The **WebModule** in **Bonyan.AspNetCore** is an extension of the standard [**[Module](/module.md)**](/module.md), designed to cater specifically to web applications. By incorporating additional lifecycle methods—**OnPreApplicationAsync**, **OnApplicationAsync**, and **OnPostApplicationAsync**—the **WebModule** provides enhanced control over the startup and operation of web applications, allowing developers to handle web-specific concerns with precision.

By leveraging both the standard module lifecycle and the additional web-specific hooks, the **WebModule** helps ensure that complex web applications can be built in a structured, modular manner, supporting clean architecture and ease of maintenance. If further guidance or examples are needed, please feel free to reach out for more details.

