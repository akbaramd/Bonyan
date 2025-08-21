# Zone-Based ViewComponent System

A robust, type-safe, loosely coupled zone-based UI component system for ASP.NET Core that allows modules to contribute UI components to specific zones with priority ordering.

## Features

- **Zone-based rendering**: Components can be assigned to specific zones
- **Priority ordering**: Components are rendered in order based on priority
- **Type-safe**: Strongly typed input models and contexts
- **Automatic discovery**: Components are automatically discovered and registered
- **Duplicate prevention**: Built-in mechanisms to prevent duplicate component rendering
- **Error isolation**: Errors in one component don't affect others
- **Flexible rendering**: Support for both HTML content and views
- **DI integration**: Full dependency injection support

## Core Concepts

### Zones
Zones are named areas in your UI where components can be rendered. Examples:
- `header`, `sidebar`, `footer`, `content`, `user-table-columns`, etc.

### Components
Components implement `IZoneComponent` and are responsible for rendering content in specific zones.

### Priority
Components have a priority value (lower numbers render first) to control rendering order within zones.

### Component IDs
Each component has a unique ID to prevent duplicate rendering. The system provides multiple strategies for ID management.

## Quick Start

### 1. Register the Zone System

```csharp
// In Program.cs or Startup.cs
services.AddZoneComponentsFrom(typeof(Program).Assembly);
```

### 2. Create a Zone Component

```csharp
public class MyZoneComponent : ZoneViewComponent<MyModel>
{
    // Custom unique ID to prevent duplicates
    public override string Id => "MyZoneComponent";

    public override IEnumerable<string> Zones => new[] { "header" };
    public override int Priority => 100;

    public override async Task<ZoneComponentResult> HandleAsync(MyModel model, ZoneRenderingContext context)
    {
        return ZoneComponentResult.Html("<div>My Content</div>");
    }
}
```

### 3. Render Zones in Views

```html
@{
    var model = new MyModel { /* ... */ };
}

<!-- Render zone with payload -->
@await Html.RenderZoneAsync("header", model)

<!-- Render zone without payload -->
@await Html.RenderZoneAsync("sidebar")
```

## Component ID Strategies

### 1. Default ID (Type Name)
```csharp
public class MyComponent : ZoneViewComponent<MyModel>
{
    // Uses GetType().Name as ID
    // Result: "MyComponent"
}
```

### 2. Custom Static ID
```csharp
public class MyComponent : ZoneViewComponent<MyModel>
{
    public override string Id => "MyCustomComponent";
    // Result: "MyCustomComponent"
}
```

### 3. Dynamic ID Based on Context
```csharp
public class MyComponent : ZoneViewComponent<MyModel, string>
{
    private readonly string _context;

    public MyComponent(string context = "default")
    {
        _context = context;
    }

    public override string Id => $"MyComponent_{_context}";
    // Result: "MyComponent_header", "MyComponent_footer", etc.
}
```

### 4. Instance-Based ID
```csharp
public class MyComponent : ZoneViewComponent<MyModel>
{
    private readonly string _instanceId;

    public MyComponent()
    {
        _instanceId = Guid.NewGuid().ToString("N")[..8];
    }

    public override string Id => $"MyComponent_{_instanceId}";
    // Result: "MyComponent_a1b2c3d4" (unique per instance)
}
```

## Duplicate Prevention

The system provides multiple layers of duplicate prevention:

### 1. Registration Level
- Components with duplicate IDs are skipped during registration
- Console logging shows which components are registered or skipped

### 2. Runtime Level
- Components are tracked per request to prevent duplicate rendering
- Each component can only be rendered once per zone per request

### 3. Manual Control
```csharp
// Clear rendered components tracking for current request
Html.ClearRenderedComponents();

// Check if component is registered
var registry = HttpContext.RequestServices.GetRequiredService<IZoneRegistry>();
var isRegistered = registry.IsRegistered("MyComponent");

// Get all registered component IDs
var componentIds = registry.GetRegisteredComponentIds();
```

## Advanced Usage

### Component with Context

```csharp
public class UserProfileComponent : ZoneViewComponent<User, UserProfileContext>
{
    public override string Id => "UserProfileComponent";
    public override IEnumerable<string> Zones => new[] { "user-profile" };
    public override int Priority => 10;

    public override async Task<ZoneComponentResult> HandleAsync(User user, ZoneRenderingContext context, UserProfileContext profileContext)
    {
        var html = await context.RenderPartialViewAsHtmlAsync("_UserProfile", new { User = user, Context = profileContext });
        return ZoneComponentResult.Html(html);
    }
}
```

### Using Views

```csharp
public class MyViewComponent : ZoneViewComponent<MyModel>
{
    public override string Id => "MyViewComponent";
    public override IEnumerable<string> Zones => new[] { "content" };
    public override int Priority => 50;

    public override async Task<ZoneComponentResult> HandleAsync(MyModel model, ZoneRenderingContext context)
    {
        return ZoneComponentResult.View("_MyPartialView", model);
    }
}
```

### Stopping Processing

```csharp
public override async Task<ZoneComponentResult> HandleAsync(MyModel model, ZoneRenderingContext context)
{
    if (someCondition)
    {
        return ZoneComponentResult.Stop(); // Stop processing other components
    }
    
    return ZoneComponentResult.Html("<div>Content</div>");
}
```

### Generic Rendering

```csharp
// Render only specific component types
@await Html.RenderZoneAsync<MyModel, MyComponent>("zone", model)

// Render multiple component types
@await Html.RenderZoneAsync<MyModel, MyComponent1, MyComponent2>("zone", model)
```

## Service Registration

### Automatic Discovery
```csharp
// Discover from single assembly
services.AddZoneComponentsFrom(typeof(Program).Assembly);

// Discover from multiple assemblies
services.AddZoneComponentsFrom(
    typeof(Program).Assembly,
    typeof(SomeModule).Assembly
);
```

### Manual Registration
```csharp
// Register as scoped (default)
services.AddZoneComponent<MyComponent>();

// Register as singleton
services.AddZoneComponentSingleton<MyComponent>();

// Register specific ZoneViewComponent types
services.AddZoneViewComponent<MyModel, MyComponent>();
services.AddZoneViewComponent<MyModel, MyContext, MyComponent>();

// Register instance
services.AddZoneComponentInstance(new MyComponent());
```

## Error Handling

The system provides robust error handling:

- **Registration errors**: Logged but don't stop other components
- **Runtime errors**: Isolated per component, logged as HTML comments
- **Missing views**: Thrown with clear error messages
- **Invalid payloads**: Graceful fallback with error messages

## Best Practices

### 1. Component IDs
- Use descriptive, unique IDs
- Consider using prefixes for module-specific components
- Avoid dynamic IDs unless you need multiple instances

### 2. Zones
- Use consistent zone naming conventions
- Document zone purposes and expected content
- Consider zone hierarchy (e.g., `user-table-header`, `user-table-row`)

### 3. Priorities
- Use priority ranges (0-100 for core, 100-200 for modules, etc.)
- Document priority conventions in your project
- Avoid too many components with the same priority

### 4. Performance
- Keep components lightweight
- Use async operations appropriately
- Consider caching for expensive operations

### 5. Testing
- Test components in isolation
- Mock dependencies appropriately
- Test error scenarios

## Migration from ViewComponents

If you're migrating from ASP.NET Core ViewComponents:

1. **Replace inheritance**: Change from `ViewComponent` to `ZoneViewComponent<T>`
2. **Update method signature**: Change from `InvokeAsync` to `HandleAsync`
3. **Add zone configuration**: Implement `Zones` and `Priority` properties
4. **Add unique ID**: Override `Id` property to prevent duplicates
5. **Update registration**: Use zone system registration instead of ViewComponent discovery

## Example Migration

### Before (ViewComponent)
```csharp
public class UserActionsViewComponent : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(User user)
    {
        return View(user);
    }
}
```

### After (Zone Component)
```csharp
public class UserActionsZoneComponent : ZoneViewComponent<User>
{
    public override string Id => "UserActionsZoneComponent";
    public override IEnumerable<string> Zones => new[] { "user-table-actions" };
    public override int Priority => 50;

    public override async Task<ZoneComponentResult> HandleAsync(User user, ZoneRenderingContext context)
    {
        return ZoneComponentResult.View("_UserActions", user);
    }
}
```

## Troubleshooting

### Common Issues

1. **Components not rendering**
   - Check if component is registered (use `registry.IsRegistered()`)
   - Verify zone name matches exactly
   - Check component priority

2. **Duplicate rendering**
   - Ensure component has unique ID
   - Check for multiple registrations
   - Use `ClearRenderedComponents()` if needed

3. **Registration errors**
   - Check console output for registration messages
   - Verify component has parameterless constructor
   - Ensure component implements `IZoneComponent`

4. **Runtime errors**
   - Check HTML output for error comments
   - Verify payload type matches component expectations
   - Test component in isolation

### Debugging

```csharp
// Check registered components
var registry = HttpContext.RequestServices.GetRequiredService<IZoneRegistry>();
var componentIds = registry.GetRegisteredComponentIds();
foreach (var id in componentIds)
{
    Console.WriteLine($"Registered: {id}");
}

// Check components in zone
var components = registry.Get("my-zone");
foreach (var component in components)
{
    Console.WriteLine($"Zone component: {component.Id} (Priority: {component.Priority})");
}
```

This zone-based system provides a powerful, flexible, and maintainable way to build modular UI components in ASP.NET Core applications. 