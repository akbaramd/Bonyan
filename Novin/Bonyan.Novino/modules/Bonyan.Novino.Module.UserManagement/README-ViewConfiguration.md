# UserManagement Module View Configuration

This document explains how views are configured to work with embedded resources in the UserManagement module.

## Problem

The original issue was that views in the UserManagement module were not being found by the Razor view engine, even though they existed as embedded resources in the module assembly.

## Solution

### 1. Virtual File System Configuration

The module configures the virtual file system to include embedded resources:

```csharp
Configure<BonVirtualFileSystemOptions>(options =>
{
    options.FileSets.AddEmbedded<BonyanNovinoUserManagementModule>("Bonyan.Novino.Module.UserManagement", "Areas/UserManagement");
});
```

### 2. Razor View Engine Configuration (in Web Module)

The main web module (`BonyanNovinoWebModule`) configures the Razor view engine to handle embedded resources:

```csharp
private void ConfigureRazorViewEngine(BonConfigurationContext context)
{
    context.Services.Configure<RazorViewEngineOptions>(options =>
    {
        // Add view locations for embedded resources
        options.ViewLocationFormats.Add("/Areas/{2}/Views/{1}/{0}.cshtml");
        options.ViewLocationFormats.Add("/Areas/{2}/Views/Shared/{0}.cshtml");
        options.ViewLocationFormats.Add("/Views/{1}/{0}.cshtml");
        options.ViewLocationFormats.Add("/Views/Shared/{0}.cshtml");
    });
}
```

### 3. Project Configuration

The module's project file embeds all view files as resources:

```xml
<ItemGroup>
    <EmbeddedResource Include="Areas\UserManagement\Views\**\*.cshtml" />
</ItemGroup>
```

### 4. View Start Configuration

The `_ViewStart.cshtml` file references the layout from the main web project:

```cshtml
@{
    Layout = "~/Views/Shared/_Layout.cshtml";
}
```

## File Structure

```
modules/Bonyan.Novino.Module.UserManagement/
├── Areas/
│   └── UserManagement/
│       ├── Controllers/
│       │   └── UserController.cs
│       ├── Models/
│       │   └── UserManagementViewModels.cs
│       └── Views/
│           ├── _ViewImports.cshtml
│           ├── _ViewStart.cshtml
│           └── User/
│               └── Index.cshtml
├── BonyanNovinoUserManagementModule.cs
└── Bonyan.Novino.Module.UserManagement.csproj
```

## Key Points

1. **Virtual File System**: Embeds the module's views as resources
2. **Razor View Engine**: Configured in the main web module to handle embedded resources
3. **Area Routing**: Properly configured for area-based routing
4. **Layout Reference**: Views reference the main web project's layout
5. **Project References**: Module has necessary project references but not circular dependencies

## Testing

To test the configuration:

1. Build the solution
2. Navigate to `http://localhost:5150/UserManagement/User/Index`
3. The view should load properly from the embedded resources

## Troubleshooting

If views are still not found:

1. Check that the virtual file system configuration is correct
2. Verify that views are properly embedded as resources
3. Ensure the Razor view engine configuration is in the main web module
4. Check that the area routing is properly configured
5. Verify that the layout file path is correct 