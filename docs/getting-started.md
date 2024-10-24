# Bonyan Library Installation and Setup Guide

Welcome to the Bonyan Library! This guide will walk you through installing and configuring the `Bonyan` and `Bonyan.AspNetCore` packages in your .NET Core project. Below, you'll find multiple methods to install the packages and set up a Bonyan module structure for your application.

## Installation Methods

You can install the `Bonyan` packages using any of the following methods:

### 1. .NET CLI

Use the .NET Command Line Interface to add the `Bonyan.AspNetCore` package to your project:

```bash
dotnet add package Bonyan.AspNetCore
```

### 2. Package Manager Console (Visual Studio)

Alternatively, you can use the Package Manager Console available in Visual Studio:

```powershell
Install-Package Bonyan.AspNetCore
```

### 3. PackageReference

Another option is to manually add the package reference in your project `.csproj` file:

```xml
<ItemGroup>
  <PackageReference Include="Bonyan.AspNetCore" Version="latest" />
</ItemGroup>
```

Make sure to replace `latest` with the version of the package you want to use.

## Using Bonyan in Your Project

Once the `Bonyan` package is installed, you can configure it in your project by setting up an application builder. Here's a sample code snippet to get you started:

```csharp
var builder = BonyanApplication
    .CreateApplicationBuilder<BonyanTemplateModule>(args);

var app = builder.Build();

app.Run();
```

This will create and configure a basic Bonyan application.

## Module Structure

In `Bonyan`, each module should inherit from `Module` (or `WebModule` for web-related modules). A module encapsulates a specific part of the application, making it more maintainable and modular. Below is a basic example of how you can create a module:

```csharp
namespace BonyanTemplate.Api
{
    public class BonyanTemplateModule : Module
    {
        public override Task OnConfigureAsync(ModularityContext context)
        {
            // Custom configuration code here
            return base.OnConfigureAsync(context);
        }

        public override Task OnInitializeAsync(ModularityInitializedContext context)
        {
            // Custom initialization code here
            return base.OnInitializeAsync(context);
        }
    }
}
```

## Specifying Dependencies with `DependOnAttribute`

Bonyan allows you to define module dependencies using the `DependOnAttribute`. This attribute helps in managing dependencies between various modules, ensuring that dependent modules are loaded in the correct order.

Here’s how you can specify a dependency between modules:

```csharp
using Bonyan.Modularity;

namespace BonyanTemplate.Api
{
    [DependOn(typeof(AnotherModule))]
    public class BonyanTemplateModule : Module
    {
        public override Task OnConfigureAsync(ModularityContext context)
        {
            // Module configuration code
            return base.OnConfigureAsync(context);
        }
    }
}
```

In this example, `BonyanTemplateModule` depends on `AnotherModule`, meaning `AnotherModule` will be initialized before `BonyanTemplateModule`. This is particularly useful for managing complex applications that contain multiple interdependent modules.

## Summary

- Install `Bonyan.AspNetCore` using CLI, Visual Studio, or by adding it to your `.csproj` file.
- Create a custom module by inheriting from `Module` or `WebModule`.
- Use `DependOnAttribute` to manage dependencies between your modules.

For more detailed information, please refer to the official documentation or contact the maintainers of the Bonyan library. We hope this guide helps you get started with Bonyan!

## Example Console Output

Here is an example of what the console output might look like when modules with dependencies are created:

```
- BonyanTemplateModule created
  - AnotherModule created
    - Another2Module created
```

This output shows the order in which the modules are initialized, respecting their dependencies.

