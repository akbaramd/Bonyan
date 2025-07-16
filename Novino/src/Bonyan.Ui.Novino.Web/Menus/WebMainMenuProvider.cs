using System.Security.Claims;
using Bonyan.Novino.Core.Menus;

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
            "sidebar-main",
            "sidebar-system",
            "topbar-user"
        };

        public override async Task<IEnumerable<MenuItem>> GetMenuItemsAsync(string location, ClaimsPrincipal? user = null)
        {

            return location.ToLowerInvariant() switch
            {
                "sidebar-main" => GetSidebarMainItems(user),
                "topbar-user" => GetTopbarUserItems(user),
                _ => Enumerable.Empty<MenuItem>()
            };
        }

        private IEnumerable<MenuItem> GetSidebarMainItems(ClaimsPrincipal? user)
        {
            var items = new List<MenuItem>
            {
                new MenuItem("داشبورد", "/", "ri-dashboard-2-line", 1)
                {
                    CssClass = "nav-link menu-link",
                    Children = new List<MenuItem>
                    {
                        new MenuItem("نمای کلی", "/Dashboard", "ri-home-line", 1)
                        {
                            CssClass = "nav-link"
                        }
                       
                    }
                }
            };

            return items;
        }


        private IEnumerable<MenuItem> GetTopbarUserItems(ClaimsPrincipal? user)
        {
            var items = new List<MenuItem>();

          
                items.AddRange(new[]
                {
                    new MenuItem("پروفایل", "/Account/Profile", "mdi mdi-account-circle", 1)
                    {
                        CssClass = "dropdown-item"
                    },
                    new MenuItem("تنظیمات", "/Account/Settings", "mdi mdi-cog-outline", 2)
                    {
                        CssClass = "dropdown-item"
                    },
                    new MenuItem("خروج", "/Account/Logout", "mdi mdi-logout", 3)
                    {
                        CssClass = "dropdown-item text-danger"
                    }
                });
          
           

            return items;
        }
    }
} 