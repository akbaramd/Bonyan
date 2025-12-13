# Bonyan.AspNetCore - Issues Summary

## 🔴 Critical Bugs (Must Fix)

### 1. Async Deadlock in WebModularityApplication
**Location**: `WebModularityApplication.cs:42-47`

```csharp
// ❌ CURRENT (DEADLOCK RISK)
ExecuteModulePhaseAsync(webModules, (module) => module.OnPreApplicationAsync(context), "Pre-Application")
    .GetAwaiter().GetResult();
```

**Fix**:
```csharp
// ✅ FIXED
await ExecuteModulePhaseAsync(webModules, (module) => module.OnPreApplicationAsync(context), "Pre-Application");
```

---

### 2. NotImplementedException
**Location**: `BonyanApplicationBuilder.cs:29-32`

```csharp
// ❌ CURRENT
public Task<WebApplication> BuildAsync()
{
    throw new NotImplementedException();
}
```

**Fix**: Remove this overload or delegate to the parameterized version.

---

### 3. Service Scope Disposed Too Early
**Location**: `BonAspNetCoreModule.cs:141-153`

```csharp
// ❌ CURRENT (SCOPE DISPOSED BEFORE USE)
context.Application.UseEndpoints(endpoints =>
{
    using var scope = context.Application.Services.CreateScope();
    // Scope is disposed when UseEndpoints returns, but endpoints execute later!
    var endpointRouteBuilderContext = new EndpointRouteBuilderContext(endpoints, scope.ServiceProvider);
    // ...
});
```

**Fix**: Don't create scope here - endpoints create their own scopes per request.

---

### 4. Typo in Parameter Name
**Location**: `WebModularityApplication.cs:29`, `IWebModularityApplication.cs:5`

```csharp
// ❌ CURRENT
public async Task InitializeApplicationAsync(WebApplication application, Action<BonWebApplicationContext>? applciationContext = null)
```

**Fix**: Rename `applciationContext` → `applicationContext`

---

### 5. Missing Null Checks
**Multiple Locations**

```csharp
// ❌ CURRENT
var options = context.Application.Services.GetRequiredService<IOptions<BonEndpointRouterOptions>>().Value;
// No null check, could throw

// ✅ FIXED
var options = context.Application.Services.GetService<IOptions<BonEndpointRouterOptions>>()?.Value;
if (options == null) throw new InvalidOperationException("BonEndpointRouterOptions not configured");
```

---

## 🟡 Architecture Problems

### 6. No Fluent API
**Problem**: Configuration is verbose and not chainable.

**Current**:
```csharp
var builder = BonyanApplication.CreateBuilder("service");
builder.Services.AddLogging();
builder.Services.Configure<ExceptionHandlingOptions>(...);
// etc.
```

**Should Be**:
```csharp
var builder = BonyanApplication.CreateBuilder("service")
    .ConfigureLogging(logging => logging.AddConsole())
    .ConfigureExceptionHandling(options => options.ApiExceptionMiddlewareEnabled = false)
    .ConfigureUnitOfWork(options => options.IgnoredUrls.Add("/health"));
```

---

### 7. Inconsistent Return Types
**Problem**: Mix of `Task` and `ValueTask`.

**Fix**: Standardize on `ValueTask` for all lifecycle methods.

---

### 8. Missing CancellationToken
**Problem**: No cancellation support.

**Fix**: Add `CancellationToken` to all async methods.

---

### 9. Hardcoded Dependencies
**Problem**: Autofac hardcoded, Newtonsoft.Json hardcoded.

**Fix**: Use abstractions, support multiple implementations.

---

## 📋 Complete Issue List

See `ARCHITECTURE_ANALYSIS_REPORT.md` for full details of all 47 issues.

