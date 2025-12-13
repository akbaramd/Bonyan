# Bonyan Modularity System: Architecture Guide

## System Overview

Module-based framework for .NET enabling:
- **Modular Architecture**: Independent, composable modules
- **Dependency Management**: Automatic resolution and topological sorting
- **Lifecycle Orchestration**: 6-phase lifecycle (3 config + 3 init phases)
- **Plugin Support**: Dynamic discovery from JSON manifests and folders
- **Clean Separation**: Each module encapsulates domain functionality

**Philosophy**: "Monolithic Modularity" - microservices-like organization within single deployable unit.

---

## Architecture Components

### Core Interfaces

**`IBonModule`**: Module contract with `Services`, `DependedModules`, `DependOn<T>()`, and lifecycle methods (PreConfigure, Configure, PostConfigure, PreInitialize, Initialize, PostInitialize).

**`BonModule`**: Base abstract class with virtual lifecycle hooks, options helpers (`Configure<TOptions>()`, `PreConfigure`, `PostConfigure`), dependency management, and disposal pattern.

### Module Discovery & Loading

**`BonModuleLoader`**: Discovery engine with 4 responsibilities:
1. Module Discovery: Recursively finds all modules from root
2. Dependency Resolution: Builds graph from attributes, properties, providers
3. Topological Sorting: Orders modules (dependencies before dependents)
4. Plugin Integration: Loads from `PlugInSourceList`

**Process**: `LoadModules()` → `GetDescriptors()` (FillModules + SetDependencies) → `SortByDependency()`

**`BonyanModuleHelper`**: Static utility for:
- `FindAllModuleTypes()`: Recursive discovery
- `FindDependedModuleTypes()`: Extracts via attributes `[DependsOn]`, instance properties, or `IDependedTypesProvider`
- `GetAllAssemblies()`: Collects assemblies including `[AdditionalAssembly]`

### Module Descriptor

**`BonModuleDescriptor`**: Metadata container with `ModuleType`, `Instance`, `Dependencies`, `AllAssemblies`, `IsLoaded`, `ServiceName`.

### Application Orchestrator

**`BonModularityApplication<TModule>`**: Lifecycle manager implementing `IBonModuleConfigurator`, `IBonModuleInitializer`, `IBonModuleContainer`, `IBonServiceProviderAccessor`.

**Lifecycle**:
```
Constructor: InitializePluginSources → LoadModules → ConfigureModulesAsync → RegisterCoreServices
ConfigureModulesAsync: OnPreConfigureAsync → OnConfigureAsync → OnPostConfigureAsync (all in dependency order)
InitializeModulesAsync: OnPreInitializeAsync → OnInitializeAsync → OnPostInitializeAsync (all in dependency order)
```

### Context Objects

**`BonConfigurationContext`**: Config phase with `Services` (IServiceCollection), `ConfigureOptions<T>()`, `ConfigureAndValidate<T>()`, `PlugInSources`.

**`BonInitializedContext`**: Init phase with `GetOptionAsync<T>()`, `RequiredValidatedOption<T>()`, service resolution.

**`BonContextBase`**: Shared base with `Services` (IServiceProvider), `Configuration`, `GetService<T>()`, `GetRequiredService<T>()`, `GetOption<T>()`, `GetRequiredOption<T>()`, `GetServices<T>()`.

---

## Module Lifecycle

### Configuration Phases (Before ServiceProvider Build)
1. **PreConfigure**: Early setup, infrastructure
2. **Configure**: Main config, service registration
3. **PostConfigure**: Final adjustments, cross-module config

**Execution**: Dependency order (dependencies first). Root module last.

### Initialization Phases (After ServiceProvider Build)
1. **PreInitialize**: Early init, setup dependent services
2. **Initialize**: Main init, start background services
3. **PostInitialize**: Final init, cross-module coordination

**Execution**: Same dependency-ordered pattern.

---

## Dependency Resolution

### Declaration Methods
1. Attribute: `[DependsOn(typeof(ModuleA), typeof(ModuleB))]`
2. Instance: `DependedModules = new List<Type> { typeof(ModuleA) }`
3. Fluent: `DependOn<ModuleA>(); DependOn(typeof(ModuleB), typeof(ModuleC));`

### Resolution Process
1. Extract dependencies (attributes + instance + providers)
2. Find `BonModuleDescriptor` for each
3. Validate existence (throws `BonException` if missing)
4. Build dependency graph
5. Topological sort (dependencies before dependents)
6. Move root module to end

**Example**: ModuleA → [], ModuleB → [ModuleA], ModuleC → [ModuleA, ModuleB], RootModule → [ModuleC]
**Sorted**: [ModuleA, ModuleB, ModuleC, RootModule]

---

## Plugin System

**Discovery**: `JsonPluginSource` (JSON manifests), `FolderPlugInSource` (folders), or custom `IPlugInSource`.

**Manifest**: Name, Version, Authors, Description, EntryPoint, AdditionalFiles, Tags.

**Initialization**: Populated during app creation via `creationContext`, used during module loading. Plugin modules marked `isLoadedAsPlugIn: true`.

---

## Service Registration

**Core Services** (all singletons): `IBonModuleLoader`, `IBonModuleContainer`, `IBonModuleConfigurator`, `IBonModuleInitializer`, `IBonModularityApplication`, `IAssemblyFinder`, `ITypeFinder`, `PlugInSourceList`.

Each module receives `IServiceCollection` during configuration for its own registrations.

---

## Design Patterns

1. **Template Method**: Lifecycle hooks in `BonModule`, orchestrated by `BonModularityApplication`
2. **Strategy**: `IBonModuleLoader` allows different loading strategies
3. **Factory**: `CreateModuleDescriptor()` creates consistent descriptors
4. **Facade**: `BonModularityApplication` simplifies module management
5. **Observer**: Lifecycle phases notify modules in dependency order

---

## Usage Example

```csharp
public class CoreModule : BonModule
{
    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        context.Services.AddSingleton<ICoreService, CoreService>();
        return Task.CompletedTask;
    }
}

public class FeatureModule : BonModule
{
    public FeatureModule() => DependOn<CoreModule>();
    
    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        context.Services.AddScoped<IFeatureService, FeatureService>();
        return Task.CompletedTask;
    }
}

var services = new ServiceCollection();
var app = new BonModularityApplication<FeatureModule>(
    services, "MyService",
    context => context.PlugInSources.AddFolder("Plugins")
);
await app.InitializeModulesAsync(app.ServiceProvider);
```

---

## Error Handling

- **Phase Failures**: `InvalidOperationException` with module context
- **Missing Dependencies**: `BonException` with dependency details
- **Validation Failures**: `OptionsValidationException` for invalid options

---

## Extensibility Points

1. Custom `IBonModuleLoader` implementations
2. `[AdditionalAssembly]` for multi-assembly modules
3. `IDependedTypesProvider` for custom dependency providers
4. Virtual methods in `BonModuleLoader` for customization
5. Custom `IPlugInSource` implementations

---

## Key Benefits

- **Maintainability**: Clear module boundaries
- **Testability**: Modules testable in isolation
- **Scalability**: Easy add/remove modules
- **Flexibility**: Dynamic plugin loading
- **Organization**: Correct initialization order via dependency management

Enables enterprise-grade applications with clean architecture, separation of concerns, and maintainable structure.
