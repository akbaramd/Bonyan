# Bonyan.Localization

Core localization module for the Bonyan framework that provides contracts, options, and extensions that all modules can use to configure their localization needs.

## Purpose

This module serves as the foundation for localization in the Bonyan framework. It provides:

- **Contracts**: Interfaces that define localization operations
- **Options**: Configuration classes for localization settings
- **Extensions**: Fluent API for easy configuration
- **Module Support**: Infrastructure for modules to register their localization needs

## Key Components

### BonLocalizationOptions
Main configuration class that allows modules to:
- Register module-specific localization info
- Configure resource paths
- Define resource files and keys
- Add custom configurators

### ModuleLocalizationInfo
Allows individual modules to define:
- Their specific resource paths
- Resource file mappings
- Resource keys

### IBonLocalizationManager
Core interface for localization operations:
- Get localized strings
- Culture-specific operations
- Culture management

### IBonLocalizationService
Simplified interface for common localization tasks:
- Short `L()` method for getting localized strings
- Culture management

## Usage in Modules

### Basic Module Configuration

```csharp
public class MyModule : BonModule
{
    public override Task OnPreConfigureAsync(BonConfigurationContext context)
    {
        PreConfigure<BonLocalizationOptions>(options =>
        {
            // Add module-specific localization
            options.AddModuleLocalization("MyModule", moduleInfo =>
            {
                moduleInfo.SetResourcePath("MyModule/Resources")
                         .AddCulture("en-US")
                         .AddCulture("fa-IR")
                         .AddResourceFile("Shared", "SharedResource")
                         .AddResourceKey("WelcomeMessage");
            });
        });
        
        return base.OnPreConfigureAsync(context);
    }
}
```

### Using Extension Methods

```csharp
PreConfigure<BonLocalizationOptions>(options =>
{
    options.AddSupportedCulture("en-US")
           .AddSupportedCulture("fa-IR")
           .SetDefaultCulture("fa-IR")
           .AddModuleLocalization("MyModule", moduleInfo =>
           {
               moduleInfo.SetResourcePath("MyModule/Resources")
                        .AddCulture("en-US")
                        .AddCulture("fa-IR");
           });
});
```

## Module Integration

Modules can use this core module to:

1. **Register their localization needs** - Define what cultures they support
2. **Specify resource paths** - Tell where their resource files are located
3. **Define resource keys** - Document what keys they use
4. **Configure cultures** - Set which cultures they support

The actual localization implementation (DI registration, ASP.NET Core integration) is handled by `Bonyan.AspNetCore.Localization` module.

## Dependencies

- Bonyan (Core framework)
- Bonyan.Collections (For TypeList support)

## Note

This module does NOT handle:
- Service registration (handled by Bonyan.AspNetCore.Localization)
- ASP.NET Core integration (handled by Bonyan.AspNetCore.Localization)
- Resource file loading (handled by ASP.NET Core)

It only provides the contracts and configuration infrastructure that all modules can use. 