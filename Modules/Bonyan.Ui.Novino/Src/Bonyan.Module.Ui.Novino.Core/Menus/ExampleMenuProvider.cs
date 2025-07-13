using System.Security.Claims;

namespace Menus
{
    /// <summary>
    /// Example menu provider demonstrating how to contribute menu items to different locations
    /// </summary>
    public class ExampleMenuProvider : MenuProviderBase
    {
        public override string ProviderId => "example-menu-provider";

        public override int Priority => 100;

        public override IEnumerable<string> SupportedLocations => new[]
        {
            "main-navigation",
            "sidebar",
            "admin",
            "user-profile"
        };

        public override async Task<IEnumerable<MenuItem>> GetMenuItemsAsync(string location, ClaimsPrincipal? user = null)
        {
            var menuItems = new List<MenuItem>();

            switch (location.ToLowerInvariant())
            {
                case "main-navigation":
                    menuItems.AddRange(await GetMainNavigationItems(user));
                    break;
                
                case "sidebar":
                    menuItems.AddRange(await GetSidebarItems(user));
                    break;
                
                case "admin":
                    menuItems.AddRange(await GetAdminItems(user));
                    break;
                
                case "user-profile":
                    menuItems.AddRange(await GetUserProfileItems(user));
                    break;
            }

            return menuItems;
        }

        private async Task<IEnumerable<MenuItem>> GetMainNavigationItems(ClaimsPrincipal? user)
        {
            await Task.Delay(1); // Simulate async operation
            
            var items = new List<MenuItem>
            {
                new MenuItem("Home", "/", "fas fa-home", 1),
                new MenuItem("Products", "/products", "fas fa-shopping-cart", 2),
                new MenuItem("Services", "/services", "fas fa-cogs", 3),
                new MenuItem("About", "/about", "fas fa-info-circle", 4),
                new MenuItem("Contact", "/contact", "fas fa-envelope", 5)
            };

            // Add submenu for Products
            var productsItem = items.First(i => i.Title == "Products");
            productsItem.AddChild(new MenuItem("All Products", "/products", "fas fa-list", 1));
            productsItem.AddChild(new MenuItem("Categories", "/products/categories", "fas fa-folder", 2));
            productsItem.AddChild(new MenuItem("Featured", "/products/featured", "fas fa-star", 3));

            return items;
        }

        private async Task<IEnumerable<MenuItem>> GetSidebarItems(ClaimsPrincipal? user)
        {
            await Task.Delay(1); // Simulate async operation
            
            return new List<MenuItem>
            {
                new MenuItem("Dashboard", "/dashboard", "fas fa-tachometer-alt", 1),
                new MenuItem("Quick Actions", "#", "fas fa-bolt", 2)
                {
                    Children = new List<MenuItem>
                    {
                        new MenuItem("New Order", "/orders/new", "fas fa-plus", 1),
                        new MenuItem("New Customer", "/customers/new", "fas fa-user-plus", 2),
                        new MenuItem("New Product", "/products/new", "fas fa-box", 3)
                    }
                },
                new MenuItem("Reports", "/reports", "fas fa-chart-bar", 3)
                {
                    RequiresAuthentication = true,
                    RequiredRoles = new List<string> { "Manager", "Admin" }
                }
            };
        }

        private async Task<IEnumerable<MenuItem>> GetAdminItems(ClaimsPrincipal? user)
        {
            await Task.Delay(1); // Simulate async operation
            
            return new List<MenuItem>
            {
                new MenuItem("User Management", "/admin/users", "fas fa-users", 1)
                {
                    RequiredRoles = new List<string> { "Admin" },
                    Children = new List<MenuItem>
                    {
                        new MenuItem("All Users", "/admin/users", "fas fa-list", 1),
                        new MenuItem("Add User", "/admin/users/create", "fas fa-user-plus", 2),
                        new MenuItem("Roles", "/admin/roles", "fas fa-user-tag", 3)
                    }
                },
                new MenuItem("System Settings", "/admin/settings", "fas fa-cog", 2)
                {
                    RequiredRoles = new List<string> { "Admin" }
                },
                new MenuItem("Logs", "/admin/logs", "fas fa-file-alt", 3)
                {
                    RequiredRoles = new List<string> { "Admin" }
                }
            };
        }

        private async Task<IEnumerable<MenuItem>> GetUserProfileItems(ClaimsPrincipal? user)
        {
            await Task.Delay(1); // Simulate async operation
            
            if (user == null || !user.Identity?.IsAuthenticated == true)
            {
                return new List<MenuItem>
                {
                    new MenuItem("Login", "/login", "fas fa-sign-in-alt", 1),
                    new MenuItem("Register", "/register", "fas fa-user-plus", 2)
                };
            }

            return new List<MenuItem>
            {
                new MenuItem("Profile", "/profile", "fas fa-user", 1),
                new MenuItem("Settings", "/settings", "fas fa-cog", 2),
                new MenuItem("Notifications", "/notifications", "fas fa-bell", 3)
                {
                    VisibilityCondition = (u) => u?.FindFirst("notifications_enabled")?.Value == "true"
                },
                new MenuItem("Logout", "/logout", "fas fa-sign-out-alt", 4)
            };
        }
    }
} 