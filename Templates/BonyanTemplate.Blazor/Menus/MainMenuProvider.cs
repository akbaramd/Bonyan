using Bonyan.AspNetCore.Components.Menus;
using BonyanTemplate.Blazor.Consts;

namespace BonyanTemplate.Blazor.Menus
{
    public class MainMenuProvider : IMenuProvider
    {
        public IEnumerable<MenuItem> GetMenuItems(string location)
        {
            if (location.Equals(AdminLteMenuConst.NavbarMenu, StringComparison.OrdinalIgnoreCase))
            {
                return new List<MenuItem>
                {
                    new MenuItem
                    {
                        Title = "صفحه نخست",
                        Url = "/dashboard",
                        Order = 1,
                        MetaData = new Dictionary<string, string>
                        {
                            { "align", "left" }
                        }
                    },
                    new MenuItem
                    {
                        Title = "تماس با ما",
                        Url = "/contact",
                        Order = 2,
                        MetaData = new Dictionary<string, string>
                        {
                            { "align", "left" }
                        }
                    }
                };
            }

            
            if (location.Equals(AdminLteMenuConst.SidebarMenu, StringComparison.OrdinalIgnoreCase))
            {
                return new List<MenuItem>
                {
                    new MenuItem
                    {
                        Title = "صفحه نخست",
                        Url = "/dashboard",
                        Order = 1, // Set the desired order
                        MetaData = new Dictionary<string, string>
                        {
                            { "customKey", "customValue" }
                        }
                    }
                };
            }

            // Return an empty list for other locations
            return Enumerable.Empty<MenuItem>();
        }
    }
}