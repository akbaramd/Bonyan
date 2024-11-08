namespace Bonyan.AspNetCore.Components
{
    public class MenuService
    {
        private readonly Dictionary<string, Menu> _menus = new();
        private readonly Dictionary<string, string> _menuLocations = new();

        // Add a menu
        public void AddMenu(string menuName, Menu menu)
        {
            if (!_menus.ContainsKey(menuName))
            {
                _menus[menuName] = menu;
            }
        }

        // Remove a menu
        public void RemoveMenu(string menuName)
        {
            if (_menus.ContainsKey(menuName))
            {
                _menus.Remove(menuName);
            }
        }

        // Assign a menu to a location
        public void AssignMenuToLocation(string locationName, string menuName)
        {
            if (_menus.ContainsKey(menuName))
            {
                _menuLocations[locationName] = menuName;
            }
        }

        // Remove a menu from a location
        public void RemoveMenuFromLocation(string locationName)
        {
            if (_menuLocations.ContainsKey(locationName))
            {
                _menuLocations.Remove(locationName);
            }
        }

        // Get a menu assigned to a location
        public Menu GetMenuByLocation(string locationName)
        {
            if (_menuLocations.TryGetValue(locationName, out var menuName))
            {
                if (_menus.TryGetValue(menuName, out var menu))
                {
                    return menu;
                }
            }
            return null;
        }

        // Define a menu location (e.g., "sidebar", "footer")
        public void RegisterMenuLocation(string locationName)
        {
            if (!_menuLocations.ContainsKey(locationName))
            {
                _menuLocations[locationName] = null;
            }
        }

        // Get all menu locations
        public Dictionary<string, string> GetMenuLocations()
        {
            return _menuLocations;
        }
    }
}
