using System.Security.Claims;
using Menus;

namespace Bonyan.Novino.Web.Menus
{
    /// <summary>
    /// Menu provider for the main web application navigation
    /// </summary>
    public class WebMainMenuProvider : MenuProviderBase
    {
        public override string ProviderId => "web-main-menu-provider";

        public override int Priority => 1000; // High priority for main navigation

        public override IEnumerable<string> SupportedLocations => new[]
        {
            "main-navigation",
            "footer-menu",
            "user-menu"
        };

        public override async Task<IEnumerable<MenuItem>> GetMenuItemsAsync(string location, ClaimsPrincipal? user = null)
        {
            await Task.Delay(1); // Simulate async operation

            return location.ToLowerInvariant() switch
            {
                "main-navigation" => GetMainNavigationItems(user),
                "footer-menu" => GetFooterMenuItems(user),
                "user-menu" => GetUserMenuItems(user),
                _ => Enumerable.Empty<MenuItem>()
            };
        }

        private IEnumerable<MenuItem> GetMainNavigationItems(ClaimsPrincipal? user)
        {
            var items = new List<MenuItem>
            {
                new MenuItem("Home", "/", "fas fa-home", 1)
                {
                    CssClass = "nav-link text-dark"
                },
                new MenuItem("Privacy", "/Home/Privacy", "fas fa-shield-alt", 2)
                {
                    CssClass = "nav-link text-dark"
                }
            };

            // Add authenticated user items
            if (user?.Identity?.IsAuthenticated == true)
            {
                items.Add(new MenuItem("Dashboard", "/Dashboard", "fas fa-tachometer-alt", 3)
                {
                    CssClass = "nav-link text-dark",
                    RequiresAuthentication = true
                });

                // Add admin menu for admin users
                if (user.IsInRole("Admin"))
                {
                    var adminItem = new MenuItem("Admin", "#", "fas fa-cog", 4)
                    {
                        CssClass = "nav-link text-dark",
                        RequiredRoles = new List<string> { "Admin" }
                    };
                    
                    adminItem.AddChild(new MenuItem("Users", "/Admin/Users", "fas fa-users", 1));
                    adminItem.AddChild(new MenuItem("Settings", "/Admin/Settings", "fas fa-cogs", 2));
                    adminItem.AddChild(new MenuItem("Logs", "/Admin/Logs", "fas fa-file-alt", 3));
                    
                    items.Add(adminItem);
                }
            }

            return items;
        }

        private IEnumerable<MenuItem> GetFooterMenuItems(ClaimsPrincipal? user)
        {
            return new List<MenuItem>
            {
                new MenuItem("Privacy", "/Home/Privacy", "", 1)
                {
                    CssClass = "footer-link"
                },
                new MenuItem("Terms", "/Home/Terms", "", 2)
                {
                    CssClass = "footer-link"
                },
                new MenuItem("Contact", "/Home/Contact", "", 3)
                {
                    CssClass = "footer-link"
                }
            };
        }

        private IEnumerable<MenuItem> GetUserMenuItems(ClaimsPrincipal? user)
        {
            if (user?.Identity?.IsAuthenticated != true)
            {
                return new List<MenuItem>
                {
                    new MenuItem("Login", "/Account/Login", "fas fa-sign-in-alt", 1)
                    {
                        CssClass = "btn btn-outline-primary"
                    },
                    new MenuItem("Register", "/Account/Register", "fas fa-user-plus", 2)
                    {
                        CssClass = "btn btn-primary ms-2"
                    }
                };
            }

            return new List<MenuItem>
            {
                new MenuItem($"Hello {user.Identity.Name}!", "#", "fas fa-user", 1)
                {
                    CssClass = "nav-link dropdown-toggle",
                    Children = new List<MenuItem>
                    {
                        new MenuItem("Profile", "/Account/Profile", "fas fa-user", 1),
                        new MenuItem("Settings", "/Account/Settings", "fas fa-cog", 2),
                        new MenuItem("Logout", "/Account/Logout", "fas fa-sign-out-alt", 3)
                    }
                }
            };
        }
    }
} 