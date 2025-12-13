# BonConfigurationContext Fluent API Guide

## Overview

The `BonConfigurationContext` fluent API provides a design-system style interface for configuring Bonyan applications in a consistent, discoverable, and chainable way.

## Usage

### Basic Setup

```csharp
var builder = BonyanApplication.CreateModularBuilder<MyModule>(
    serviceName: "my-service",
    creationContext: ctx => ctx
        // Chain fluent methods here
        .AddPluginFolder("Plugins")
        .WithOptions<MyOptions>(options => { /* ... */ })
);
```

---

## Plugin Configuration

### Add Plugin Folder

```csharp
ctx.AddPluginFolder("Plugins")
   .AddPluginFolder("CustomPlugins", autoDiscoverJson: true, searchOption: SearchOption.AllDirectories);
```

**Parameters:**
- `folderPath`: Path to the plugin folder
- `autoDiscoverJson`: Whether to automatically discover plugin.json files (default: `true`)
- `searchOption`: Search option for scanning subfolders (default: `TopDirectoryOnly`)

### Add Multiple Plugin Folders

```csharp
ctx.AddPluginFolders(
    SearchOption.AllDirectories,
    autoDiscoverJson: true,
    "Plugins", "CustomPlugins", "ThirdPartyPlugins");
```

### Add Plugin Manifest

```csharp
ctx.AddPluginManifest("plugins/plugin.json");
```

### Add Multiple Plugin Manifests

```csharp
ctx.AddPluginManifests("plugins/plugin1.json", "plugins/plugin2.json");
```

---

## Options Configuration

### Configure Options

```csharp
ctx.WithOptions<MyOptions>(options =>
{
    options.Property1 = "value1";
    options.Property2 = 42;
});
```

### Configure Options from Configuration Section

```csharp
ctx.WithOptionsFromConfiguration<MyOptions>("MyOptions");
```

This reads from `appsettings.json`:
```json
{
  "MyOptions": {
    "Property1": "value1",
    "Property2": 42
  }
}
```

### Configure and Validate Options

```csharp
ctx.WithValidatedOptions<MyOptions>(
    configure: options => options.Property1 = "value1",
    validate: options => !string.IsNullOrEmpty(options.Property1));
```

### Conditional Options Configuration

```csharp
ctx.WithOptionsIf<MyOptions>(
    condition: true,
    configure: options => options.EnableFeature = true);
```

### Environment-Specific Options

```csharp
ctx.WithOptionsForEnvironment<MyOptions>(
    environmentName: "Development",
    configure: options => options.EnableDebugMode = true);
```

---

## Service Registration

### Add Service with Lifetime

```csharp
ctx.AddService<IMyService, MyService>(ServiceLifetime.Singleton)
   .AddService<IRepository, Repository>(ServiceLifetime.Scoped);
```

### Add Singleton Service

```csharp
ctx.AddSingleton<IMyService, MyService>();
```

### Add Scoped Service

```csharp
ctx.AddScoped<IRepository, Repository>();
```

### Add Transient Service

```csharp
ctx.AddTransient<IValidator, Validator>();
```

### Add Service Instance

```csharp
var instance = new MyService();
ctx.AddInstance<IMyService>(instance);
```

### Add Service Factory

```csharp
ctx.AddFactory<IMyService>(
    factory: sp => new MyService(sp.GetRequiredService<ILogger>()),
    lifetime: ServiceLifetime.Scoped);
```

---

## Complete Example

```csharp
var builder = BonyanApplication.CreateModularBuilder<WebApiDemoModule>(
    serviceName: "web-api-demo",
    creationContext: ctx => ctx
        // Plugin configuration
        .AddPluginFolder("Plugins")
        .AddPluginFolders(SearchOption.AllDirectories, "CustomPlugins", "ThirdPartyPlugins")
        .AddPluginManifest("plugins/custom-plugin.json")
        
        // Options configuration
        .WithOptions<WebApiDemoOptions>(options =>
        {
            options.ApplicationName = "Web API Demo";
            options.Version = "1.0.0";
            options.EnableSwagger = true;
        })
        .WithOptionsFromConfiguration<DatabaseOptions>("Database")
        .WithValidatedOptions<ApiOptions>(
            configure: options => options.ApiKey = "secret-key",
            validate: options => !string.IsNullOrEmpty(options.ApiKey))
        
        // Conditional configuration
        .WithOptionsIf<WebApiDemoOptions>(
            condition: true,
            configure: options => options.EnableSwagger = true)
        .WithOptionsForEnvironment<WebApiDemoOptions>(
            environmentName: "Development",
            configure: options => options.EnableDebugMode = true)
        
        // Service registration
        .AddSingleton<IMyService, MyService>()
        .AddScoped<IRepository, Repository>()
        .AddTransient<IValidator, Validator>()
        .AddFactory<ICustomService>(
            factory: sp => new CustomService(sp.GetRequiredService<ILogger>()),
            lifetime: ServiceLifetime.Singleton)
);
```

---

## Benefits

1. **Consistency**: All configuration follows the same pattern
2. **Discoverability**: IntelliSense shows all available methods
3. **Chainability**: Methods return the context for easy chaining
4. **Readability**: Code reads like a sentence
5. **Type Safety**: Compile-time checking of options and services
6. **Validation**: Built-in support for options validation

---

## Best Practices

1. **Group Related Configuration**: Keep plugin, options, and service registration together
2. **Use Environment-Specific Configuration**: Use `WithOptionsForEnvironment` for environment-specific settings
3. **Validate Critical Options**: Use `WithValidatedOptions` for options that must be correct
4. **Use Configuration Sections**: Prefer `WithOptionsFromConfiguration` over hardcoding values
5. **Chain Logically**: Order matters - configure plugins first, then options, then services

---

## See Also

- [Bonyan Modularity System](./BONYAN_MODULARITY_SYSTEM.md)
- [Fluent API Proposal](./FLUENT_API_PROPOSAL.md)
- [Architecture Analysis Report](../Bonyan.AspNetCore/ARCHITECTURE_ANALYSIS_REPORT.md)

