using Bonyan.AspNetCore.Components.Menus;
using BonyanTemplate.Blazor.Consts;

namespace BonyanTemplate.Blazor.Menus
{
    public class UserManagementMenuProvider : IMenuProvider
    {
        public IEnumerable<MenuItem> GetMenuItems(string location)
        {
            if (location.Equals(AdminLteMenuConst.SidebarMenu, StringComparison.OrdinalIgnoreCase))
            {
                return new List<MenuItem>
                {
                    new()
                    {
                        Title = "مدیریت کاربران",
                        Url = "#",
                        Icon = "nav-icon fas fa-users",
                        Order = 2, // Set the desired order
                        Children = new List<MenuItem>
                        {
                            new()
                            {
                                Title = "لیست کاربران",
                                Url = "/users",
                                Icon = "far fa-circle nav-icon",
                                Order = 1 // Order within the parent menu
                            },
                            new()
                            {
                                Title = "افزودن کاربر",
                                Url = "/",
                                Icon = "far fa-circle nav-icon",
                                Order = 2 // Order within the parent menu
                            }
                        }
                    }
                };
            }

            // Return an empty list for other locations
            return Enumerable.Empty<MenuItem>();
        }
    }
}