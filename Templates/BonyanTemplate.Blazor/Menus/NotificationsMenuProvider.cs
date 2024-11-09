using Bonyan.AspNetCore.Components.Menus;
using BonyanTemplate.Blazor.Consts;

namespace BonyanTemplate.Blazor.Menus
{
    public class NotificationsMenuProvider : IMenuProvider
    {
        public IEnumerable<MenuItem> GetMenuItems(string location)
        {
            if (location.Equals(AdminLteMenuConst.NavbarMenu, StringComparison.OrdinalIgnoreCase))
            {
                return new List<MenuItem>
                {
                    new()
                    {
                        Title = "Notifications",
                        Url = "#",
                        Icon = "far fa-bell",
                        Order = 1,
                        MetaData = new Dictionary<string, string>
                        {
                            { "align", "right" }
                        },
                        Children = new List<MenuItem>
                        {
                            new()
                            {
                                Title = "New Message",
                                Url = "/messages/new",
                                Icon = "fas fa-envelope",
                                Order = 1
                            },
                            new()
                            {
                                Title = "Profile Update",
                                Url = "/profile/update",
                                Icon = "fas fa-user",
                                Order = 2
                            }
                        }
                    }
                };
            }

            return Enumerable.Empty<MenuItem>();
        }
    }
}