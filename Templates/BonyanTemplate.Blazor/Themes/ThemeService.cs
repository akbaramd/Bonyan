namespace Bonyan.AspNetCore.Components
{
    public class ThemeService
    {
        public MenuService MenuService { get; }

        public ThemeService()
        {
            MenuService = new MenuService();
        }

        // Additional theme-related services and properties can be added here
    }
}