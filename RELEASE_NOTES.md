# Release Notes

## Version 1.6.0 (2024)

### 🎯 Major Changes

#### Configuration Context Separation
- **Breaking Change**: Introduced phase-specific configuration contexts for better architectural control:
  - `BonPreConfigurationContext`: Available only during `OnPreConfigureAsync` phase
  - `BonConfigurationContext`: Available only during `OnConfigureAsync` phase  
  - `BonPostConfigurationContext`: Available only during `OnPostConfigureAsync` phase
- Each context now exposes only the relevant configuration methods for its phase
- This enforces stricter access control and prevents misuse of configuration methods

#### Dependency Declaration Refactoring
- **Breaking Change**: Removed support for `[DependsOn]` attribute-based dependency declaration
- **Migration Required**: All modules must now use constructor-based dependency declaration:
  ```csharp
  public MyModule()
  {
      DependOn<OtherModule>();
  }
  ```
- This provides a more explicit, code-driven approach to module dependencies

### ✨ New Features

#### Enhanced Module Discovery
- Implemented recursive module discovery starting from the root entry point
- Modules are now discovered and instantiated based on constructor-based dependencies
- Improved dependency graph visualization with tree structure starting from root module

#### Improved Dependency Graph Visualization
- Enhanced module dependency graph logging with:
  - Tree visualization starting from entry point
  - Load order display
  - Clear module type indicators (Root/Entry Point, Plugin)
- Improved banner UI with precise border calculations and alignment

#### Module Loading Improvements
- Fixed `LoggerFactory` disposal issue by creating loggers before disposing temporary provider
- Fixed `IBonUnitOfWorkManager` resolution by making middleware registration conditional
- Improved module lifecycle management

### 🔧 Bug Fixes

- Fixed `System.ObjectDisposedException` for `LoggerFactory` during module initialization
- Fixed `System.InvalidOperationException` for `IBonUnitOfWorkManager` service resolution
- Fixed `BonInitializedContext` registration issue
- Fixed missing `BonPreConfigureActionList<>` type reference
- Fixed context type mismatches in various modules

### 📦 Module Updates

#### Updated Modules
- `BonAspNetCoreModule`: Updated to use phase-specific contexts, removed domain layer dependency
- `BonAspNetCoreMvcModule`: Updated to use new context types, improved view location handling
- `BonLayerDomainModule`: Updated to use `BonPostConfigurationContext`, removed mediator dependency
- `BonValidationModule`: Updated to use `BonPreConfigurationContext`
- `BonMultiTenantModule`: Updated to use `BonPostConfigurationContext`
- `WebApiDemoModule`: Updated to use phase-specific contexts, added `ProductModule` dependency

#### Architectural Improvements
- Decoupled presentation/infrastructure layers from domain layer
- Made `IBonMediator` optional in `BonDomainEventDispatcher` for better flexibility
- Improved separation of concerns across module layers

### 📝 Documentation

- Added explanation for `ExecutePreConfiguredActions` method usage
- Improved code comments and XML documentation

### ⚠️ Migration Guide

#### For Module Developers

1. **Update Configuration Methods**:
   ```csharp
   // Before
   public override ValueTask OnPreConfigureAsync(BonConfigurationContext context, ...)
   
   // After
   public override ValueTask OnPreConfigureAsync(BonPreConfigurationContext context, ...)
   ```

2. **Remove Attribute-Based Dependencies**:
   ```csharp
   // Before
   [DependsOn(typeof(OtherModule))]
   public class MyModule : BonModule { }
   
   // After
   public class MyModule : BonModule
   {
       public MyModule()
       {
           DependOn<OtherModule>();
       }
   }
   ```

3. **Update Context Method Calls**:
   ```csharp
   // OnPreConfigureAsync - Only PreConfigure available
   context.PreConfigure<TOptions>(...);
   
   // OnConfigureAsync - Only Configure available
   context.ConfigureOptions<TOptions>(...);
   
   // OnPostConfigureAsync - Only PostConfigure available
   context.PostConfigure<TOptions>(...);
   ```

### 🔄 Dependencies

- No external dependency changes
- All changes are internal architectural improvements

---

## Previous Versions

### Version 1.5.7
- Initial stable release with basic modularity support

