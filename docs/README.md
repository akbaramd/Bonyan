Here’s an enhanced **Quick Start** guide in Markdown, incorporating your project details for a comprehensive introduction.

---

# Quick Start

Welcome to the Quick Start guide for **Bonyan.AspNetCore**. This guide will walk you through setting up and using Bonyan’s modular architecture, making it easy to manage dependencies in a monolithic .NET Core application.

## Prerequisites

To begin, ensure you have:

- **.NET SDK** (version 8.0 or higher)
- **Bonyan.AspNetCore** package installed

## Installation

Add the **Bonyan.AspNetCore** package via the NuGet Package Manager Console:

```bash
dotnet add package Bonyan.AspNetCore
```

Or, include it in your `.csproj` file as shown:

```xml
<PackageReference Include="Bonyan.AspNetCore" version="check last version" />
```

Rebuild the project to load the package.

## Setting Up Bonyan in Your .NET Core Project

Once installed, you can set up **Bonyan.AspNetCore** as the backbone of your project.

### Step 1: Creating the Application Builder

The setup begins with the `BonyanApplication` builder, where your `YourMainModule` serves as the entry point for your modular architecture.

```csharp
var builder = BonyanApplication.CreateApplicationBuilder<YourMainModule>(args);
var app = await builder.BuildAsync();
app.Run();
```

- `YourMainModule`: This module acts as the entry point, allowing you to define dependencies on other modules within your project.

### Step 2: Defining Modules and Dependencies

The modularity in **Bonyan.AspNetCore** allows for a scalable, dependency-driven structure. Here’s an example of how you can define a module with dependencies:

```csharp
public class ModuleTest : Module
{
    // Configuration Lifecycle

    public override Task OnPreConfigureAsync(ServiceConfigurationContext context)
    {
        // Code to execute before the main configuration
        return base.OnPreConfigureAsync(context);
    }

    public override Task OnConfigureAsync(ServiceConfigurationContext context)
    {
        // Core configuration logic here
        return base.OnConfigureAsync(context);
    }

    public override Task OnPostConfigureAsync(ServiceConfigurationContext context)
    {
        // Code to execute after configuration is completed
        return base.OnPostConfigureAsync(context);
    }

    // Initialization Lifecycle

    public override Task OnPreInitializeAsync(ServiceInitializationContext context)
    {
        // Code to execute before initialization
        return base.OnPreInitializeAsync(context);
    }

    public override Task OnInitializeAsync(ServiceInitializationContext context)
    {
        // Core initialization logic here
        return base.OnInitializeAsync(context);
    }

    public override Task OnPostInitializeAsync(ServiceInitializationContext context)
    {
        // Code to execute after initialization is completed
        return base.OnPostInitializeAsync(context);
    }
}

```

#### Explanation:

- **DependOn<T>**: This method lets `YourMainModule` rely on other modules (`YourAnotherModule`, `YourAnotherModule2`) to enhance modularity.
- **OnConfigureAsync**: This lifecycle method allows customization of the `ServiceConfigurationContext`, enabling additional configuration after module setup.

## Lifecycle Methods

**Bonyan.AspNetCore** supports a robust lifecycle for module configuration and initialization. Each lifecycle stage is asynchronous, allowing precise control over module behavior:

### Configuration Phase

1. **OnPreConfigureAsync**: Runs before configuration begins, useful for preliminary setups.
2. **OnConfigureAsync**: Main configuration method, ideal for setting up services and options.
3. **OnPostConfigureAsync**: Finalizes the configuration phase, typically for post-processing configurations.

### Initialization Phase

1. **OnPreInitializeAsync**: Executes prior to initialization, setting the stage for initialization tasks.
2. **OnInitializeAsync**: The core initialization logic, handling the main startup processes.
3. **OnPostInitializeAsync**: Concludes the initialization phase, finalizing setup and ensuring readiness.

---

This Quick Start should cover the essentials for getting started with **Bonyan.AspNetCore** in a .NET Core project. Let me know if you'd like more in-depth documentation on specific lifecycle stages, module dependencies, or usage examples!