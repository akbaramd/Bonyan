# Microkernel Architecture: Rules of the Road

## Governance Policy

These rules enforce modularity and prevent drift into "Big Ball of Mud" architecture. **Violations should fail the build** (enforced by CI fitness functions).

---

## Rule 1: No Circular Dependencies

**Policy**: Modules must not form circular dependency chains.

**Rationale**: Cyclic dependencies destroy modularity, prevent reuse, and accelerate architectural decay toward Big Ball of Mud.

**Enforcement**: 
- `DependencyGraphBuilder` detects cycles and throws `InvalidOperationException` with cycle path
- CI fitness function `ModuleCycleDetectionTests.Modules_must_not_have_cycles()` fails build on cycles

**Example Violation**:
```csharp
// ModuleA depends on ModuleB
// ModuleB depends on ModuleC  
// ModuleC depends on ModuleA  ❌ CYCLE!
```

**Fix**: Refactor to remove cycle (extract shared functionality, invert dependency, introduce abstraction).

---

## Rule 2: Static Metadata for Dependencies

**Policy**: Module dependencies must be declared via static metadata (`[DependsOn]` attributes), not instance properties.

**Rationale**: 
- Avoids side effects from module constructors
- Enables metadata reading without instantiation
- Supports microkernel isolation (plug-ins remain independent)

**Enforcement**: 
- `BonyanModuleHelper.FindDependedModuleTypes()` reads only from attributes
- Module instantiation for metadata reading is removed

**Correct Usage**:
```csharp
[DependsOn(typeof(IdentityModule))]
[DependsOn(typeof(TenantModule))]
public class NotificationModule : BonModule { }
```

**Incorrect Usage**:
```csharp
public class NotificationModule : BonModule
{
    public NotificationModule()
    {
        DependedModules = new List<Type> { typeof(IdentityModule) }; // ❌ Use attribute instead
    }
}
```

---

## Rule 3: Weak Coupling Across Module Boundaries

**Policy**: Strong coupling is allowed **inside** a module, but across module boundaries you keep contracts weak/stable.

**Rationale**: Connascence locality - minimize connascence crossing boundaries, maximize connascence within boundaries.

**Guidelines**:
- Modules communicate through **interfaces/contracts**, not concrete implementations
- Avoid direct references to other modules' internal types
- Use events/messaging for cross-module communication when possible

**Example**:
```csharp
// ✅ Good: Module depends on interface
public interface INotificationService { }
public class NotificationModule : BonModule
{
    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        context.Services.AddScoped<INotificationService, NotificationService>();
    }
}

// ❌ Bad: Module depends on concrete implementation from another module
public class OrderModule : BonModule
{
    // Direct reference to NotificationModule's internal class
    private NotificationService _notificationService; // ❌ Tight coupling
}
```

---

## Rule 4: No Plug-in → Plug-in Dependencies (Preferred)

**Policy**: Plug-ins should ideally be independent. If dependencies are needed, they must be declared and justified.

**Rationale**: Microkernel architecture benefits from independent plug-ins. Dependencies reduce isolation.

**Guidelines**:
- Prefer no dependencies between plug-ins
- If dependencies are necessary, use `[DependsOn]` attribute
- Document why the dependency is needed
- Consider extracting shared functionality to core

---

## Rule 5: Core Stays Small (Happy Path Only)

**Policy**: Core functionality should be minimal. Complex logic belongs in plug-ins.

**Rationale**: Microkernel pattern - small core + independent plug-ins = extensibility + isolation.

**What Belongs in Core**:
- Module discovery and loading
- Dependency graph building
- Lifecycle orchestration
- Basic service registration

**What Belongs in Plug-ins**:
- Business logic
- Domain-specific features
- Complex workflows
- Feature-specific services

---

## Rule 6: Build Provider Once, Never Mutate After

**Policy**: `IServiceCollection` must not be mutated after `BuildServiceProvider()` is called.

**Rationale**: Changes after build won't be reflected in the provider, causing confusion and bugs.

**Enforcement**:
- All registrations (including object accessors) happen before provider build
- Provider is built once and stored
- Accessor value is set after build (not collection mutation)

**Correct Pattern**:
```csharp
// 1. Register everything
_serviceCollection.AddObjectAccessor<IServiceProvider>();
_serviceCollection.AddSingleton<IMyService, MyService>();

// 2. Build once
_serviceProvider = _serviceCollection.BuildServiceProvider();

// 3. Set accessor value (not collection mutation)
_serviceCollection.GetObjectAccessor<IServiceProvider>().Value = _serviceProvider;
```

---

## Rule 7: Async Lifecycle Methods Must Be Truly Async

**Policy**: Methods marked `async` must use `await`, not `.GetAwaiter().GetResult()`.

**Rationale**: Blocking on async causes deadlocks in ASP.NET Core and defeats async benefits.

**Enforcement**:
- `InitializeModulesAsync` uses proper `await` with `CancellationToken`
- Configuration phase may use blocking for backward compatibility (marked with TODO for future refactor)

**Correct**:
```csharp
public async Task InitializeModulesAsync(IServiceProvider sp, CancellationToken ct = default)
{
    await ExecuteModulePhasesAsync(context, (module, ctx, ct) => 
        module.OnPreInitializeAsync(ctx, ct), ct);
}
```

**Incorrect**:
```csharp
public async Task InitializeModulesAsync(IServiceProvider sp)
{
    ExecuteModulePhases(...).GetAwaiter().GetResult(); // ❌ Blocking
}
```

---

## Rule 8: Structured Logging, Not Console.WriteLine

**Policy**: Use `ILogger<T>` for all logging, never `Console.WriteLine` in production code.

**Rationale**: 
- Enables log level control
- Supports structured logging
- Can be redirected/configured
- Production-ready

**Enforcement**: All `Console.WriteLine` calls replaced with `ILogger` calls.

---

## Rule 9: Module Activation via DI (ActivatorUtilities)

**Policy**: Modules must be created using `ActivatorUtilities.CreateInstance()`, not raw `Activator.CreateInstance()`.

**Rationale**: Supports constructor injection, enables module dependencies on services.

**Enforcement**: `DiModuleActivator` uses `ActivatorUtilities` for all module creation.

---

## Rule 10: Cycle Detection is Non-Negotiable

**Policy**: CI must fail the build if cycles are detected. This is a hard gate.

**Rationale**: Cycles are architectural debt that compounds over time. Early detection prevents Big Ball of Mud.

**Enforcement**: 
- `ModuleCycleDetectionTests.Modules_must_not_have_cycles()` runs in CI
- Test failure = build failure
- Exception message includes cycle path for debugging

---

## Summary

These rules enforce the microkernel architecture principles:
- **Small core** (happy path only)
- **Independent plug-ins** (minimal dependencies)
- **Governance** (cycles = build failure)
- **Isolation** (static metadata, DI activation)
- **Observability** (structured logging)

Violations should be caught early via automated tests (fitness functions) and fixed before merge.

