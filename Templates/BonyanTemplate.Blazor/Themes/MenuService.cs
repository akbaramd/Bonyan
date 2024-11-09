using Bonyan.AspNetCore.Components.Menus;

namespace BonyanTemplate.Blazor.Themes
{
    public class MenuService
    {
        public List<MenuItem> SidebarMenuItems { get; } = new List<MenuItem>();
        public List<MenuItem> NavbarMenuItems { get; } = new List<MenuItem>();

        // Add a menu item to the sidebar
        public void AddSidebarMenuItem(MenuItem menuItem)
        {
            SidebarMenuItems.Add(menuItem);
        }

        // Remove a menu item from the sidebar
        public void RemoveSidebarMenuItem(MenuItem menuItem)
        {
            SidebarMenuItems.Remove(menuItem);
        }

        // Add a menu item to the navbar
        public void AddNavbarMenuItem(MenuItem menuItem)
        {
            NavbarMenuItems.Add(menuItem);
        }

        // Remove a menu item from the navbar
        public void RemoveNavbarMenuItem(MenuItem menuItem)
        {
            NavbarMenuItems.Remove(menuItem);
        }
    }
}
