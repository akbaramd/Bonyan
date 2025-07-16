using System.Security.Claims;
using Bonyan.Novino.Core.Menus;

namespace Bonyan.Novino.Module.UserManagement.Menus
{
    /// <summary>
    /// Menu provider for UserManagement module navigation
    /// </summary>
    public class UserManagementMenuProvider : MenuProviderBase
    {
        public override string ProviderId => "user-management-menu-provider";

        public override int Priority => 2000; // High priority for user management

        public override IEnumerable<string> SupportedLocations => new[]
        {
            "sidebar-system"
        };

        public override async Task<IEnumerable<MenuItem>> GetMenuItemsAsync(string location, ClaimsPrincipal? user = null)
        {
            await Task.Delay(1); // Simulate async operation

            return location.ToLowerInvariant() switch
            {
                "sidebar-system" => GetSidebarSystemItems(user),
                _ => Enumerable.Empty<MenuItem>()
            };
        }

        private IEnumerable<MenuItem> GetSidebarSystemItems(ClaimsPrincipal? user)
        {
            var items = new List<MenuItem>
            {
                new MenuItem("مدیریت کاربران", "/UserManagement/User", "ri-user-settings-line", 1)
                {
                    CssClass = "nav-link menu-link",
                    Children = new List<MenuItem>
                    {
                        new MenuItem("لیست کاربران", "/UserManagement/User/Index", "ri-user-list-line", 1)
                        {
                            CssClass = "nav-link"
                        },
                        new MenuItem("افزودن کاربر", "/UserManagement/User/Create", "ri-user-add-line", 2)
                        {
                            CssClass = "nav-link"
                        }
                    }
                }
            };

            return items;
        }
    }
} 