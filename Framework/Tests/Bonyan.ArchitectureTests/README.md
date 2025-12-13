# Bonyan Architecture Tests

## Overview

This test suite implements **fitness functions** that enforce architectural rules for the Bonyan modularity system. These tests run in CI and fail fast when architectural violations are detected, preventing drift into "Big Ball of Mud" architecture.

## 🎯 Purpose

The tests enforce machine-checkable architectural rules:
- **Cycle Detection**: Prevents circular dependencies
- **Cross-Module Boundaries**: Enforces contracts-only references
- **Plugin Isolation**: Prevents plugin-to-plugin coupling without explicit reason
- **Database Isolation**: Prevents plugins from accessing core persistence

## 📋 Test Suite

### 1. ModuleCycleDetectionTests
**Rule**: No circular dependencies in module graph.

**What it checks**: Detects cycles in module dependency graph using DFS.

**Failure message**: Includes actionable cycle path (A → B → C → A) with fix suggestions.

### 2. CrossModuleReferenceTests
**Rule**: Cross-module references must target *.Contracts or *.Abstractions only.

**What it checks**: Validates that modules only reference contracts/abstractions from other modules, not implementation assemblies.

**Failure message**: Lists all violations with assembly names.

### 3. PluginDependencyPolicyTests
**Rule**: Plugin-to-plugin dependencies require explicit `Reason`.

**What it checks**: Ensures all plugin→plugin dependencies have a `Reason` property set on `[DependsOn]` attribute.

**Failure message**: Lists violations with module names and fix instructions.

### 4. PluginDatabaseIsolationTests
**Rule**: Plugins must not reference core persistence assemblies.

**What it checks**: Validates that plugin assemblies don't reference forbidden core persistence assemblies.

**Failure message**: Lists violations with assembly names.

## 🔧 Configuration

### ArchitectureTestConstants

Edit `ArchitectureTestConstants.cs` to customize:

- **AllowedSharedAssemblies**: Core assemblies that can be referenced by any module
- **ForbiddenCorePersistenceAssemblies**: Persistence assemblies plugins must not reference
- **PluginPrefix**: Pattern for identifying plugin assemblies
- **ContractsSuffix**: Pattern for identifying contracts-only assemblies

### Assembly Discovery

The tests automatically discover assemblies from the test output directory. To test assemblies from a different location:

```csharp
var asms = AssemblyDiscovery.LoadBonyanAssembliesFrom(@"C:\path\to\assemblies");
```

## 🚀 Running Tests

```bash
dotnet test Framework/Tests/Bonyan.ArchitectureTests
```

Or run specific test:

```bash
dotnet test Framework/Tests/Bonyan.ArchitectureTests --filter "FullyQualifiedName~ModuleCycleDetectionTests"
```

## 📝 Example Violations

### Cycle Detection
```
CYCLE DETECTED: Module dependency graph contains cycles.

Cycle path: ModuleA -> ModuleB -> ModuleC -> ModuleA

Fix options:
  1. Extract shared functionality to a common module
  2. Invert the dependency (use dependency inversion principle)
  3. Introduce an adapter/mediator pattern
  4. Refactor to remove the circular dependency
```

### Cross-Module Reference
```
CROSS-MODULE REFERENCE VIOLATIONS:

Bonyan.Identity.Application references Bonyan.Tenant.Infrastructure. 
Cross-module references must target *.Contracts or *.Abstractions only.

Fix: Reference only *.Contracts or *.Abstractions assemblies from other modules.
```

### Plugin Dependency
```
PLUGIN-TO-PLUGIN DEPENDENCY VIOLATIONS:

Bonyan.Plugin.Payments.Stripe.StripeModule depends on plugin 
Bonyan.Plugin.Payments.PayPal.PayPalModule but has no Reason.

Fix: Add Reason property to [DependsOn] attribute or refactor to remove dependency.
```

## 🔄 CI Integration

These tests should run in CI as a **hard gate** - if any test fails, the build should fail.

Example GitHub Actions:

```yaml
- name: Run Architecture Tests
  run: dotnet test Framework/Tests/Bonyan.ArchitectureTests --no-build --verbosity normal
```

## 📚 Related Documentation

- [Microkernel Rules](../../Src/Bonyan/Bonyan/Modularity/MICROKERNEL_RULES.md)
- [Modularity System](../../Src/Bonyan/Bonyan/Modularity/BONYAN_MODULARITY_SYSTEM.md)
- [Implementation Summary](../../Src/Bonyan/Bonyan/Modularity/IMPLEMENTATION_SUMMARY.md)

