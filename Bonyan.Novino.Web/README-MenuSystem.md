# Menu System Implementation

This web application uses a dynamic menu system that allows plugins and modules to contribute menu items to different locations throughout the application, similar to WordPress menu locations.

## Menu Locations

The following menu locations are registered in this application:

- **main-navigation**: Primary navigation menu in the header
- **footer-menu**: Footer navigation links
- **user-menu**: User profile and account menu (login/logout area)

## How It Works

### 1. Menu Provider Registration

The `BonyanNovinoWebModule` automatically registers:
- Menu locations during configuration
- Menu providers during service registration
- Auto-discovery of all `IMenuProvider` implementations

### 2. Main Menu Provider

The `WebMainMenuProvider` provides menu items for:
- **Main Navigation**: Home, Privacy, Dashboard (authenticated), Admin (admin role)
- **Footer Menu**: Privacy, Terms, Contact
- **User Menu**: Login/Register (anonymous) or Profile dropdown (authenticated)

### 3. Dynamic Rendering

The layout uses partial views to render menus dynamically:
- `_MenuPartial.cshtml`: Renders main navigation with dropdown support
- `_UserMenuPartial.cshtml`: Renders user-specific menu items

## Adding Custom Menu Items

### Option 1: Create a New Menu Provider

```csharp
public class MyPluginMenuProvider : MenuProviderBase
{
    public override string ProviderId => "my-plugin-menu";
    public override int Priority => 100;
    public override IEnumerable<string> SupportedLocations => new[] { "main-navigation" };
    
    public override async Task<IEnumerable<MenuItem>> GetMenuItemsAsync(string location, ClaimsPrincipal? user = null)
    {
        return new List<MenuItem>
        {
            new MenuItem("My Plugin", "/my-plugin", "fas fa-plug", 100)
            {
                RequiredRoles = new List<string> { "Admin" }
            }
        };
    }
}
```

Register it in your module:
```csharp
context.Services.AddMenuProvider<MyPluginMenuProvider>();
```

### Option 2: Extend Existing Provider

Modify the `WebMainMenuProvider` to add your custom menu items.

## Menu Item Features

- **Hierarchical Structure**: Support for nested menu items
- **Role-Based Access**: Specify required roles or permissions
- **Custom Visibility**: Use custom conditions for visibility
- **Icons**: Font Awesome icon support
- **CSS Classes**: Custom styling support
- **Priority Ordering**: Control menu item order
- **Metadata**: Store additional information

## Security

Menu items support security features:
- `RequiresAuthentication`: Show only to authenticated users
- `RequiredRoles`: Show only to users with specific roles
- `RequiredPermissions`: Show only to users with specific permissions
- `VisibilityCondition`: Custom visibility logic

## Examples

### Basic Menu Item
```csharp
new MenuItem("Home", "/", "fas fa-home", 1)
```

### Secured Menu Item
```csharp
new MenuItem("Admin Panel", "/admin", "fas fa-cog", 100)
{
    RequiredRoles = new List<string> { "Admin" },
    RequiresAuthentication = true
}
```

### Menu with Children
```csharp
var productsMenu = new MenuItem("Products", "#", "fas fa-shopping-cart", 2);
productsMenu.AddChild(new MenuItem("All Products", "/products", "fas fa-list", 1));
productsMenu.AddChild(new MenuItem("Categories", "/products/categories", "fas fa-folder", 2));
```

## Plugin Integration

Plugins automatically contribute to the menu system by:
1. Implementing `IMenuProvider`
2. Registering the provider in their module's DI container
3. The menu manager automatically discovers and registers all providers

This creates a completely extensible menu system where any plugin can contribute navigation elements without modifying core application code. 