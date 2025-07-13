using System.Security.Claims;

namespace Menus
{
    /// <summary>
    /// Defines a contract for menu providers that can contribute menu items to the application
    /// </summary>
    public interface IMenuProvider
    {
        /// <summary>
        /// Gets the unique identifier for this menu provider
        /// </summary>
        string ProviderId { get; }

        /// <summary>
        /// Gets the priority of this menu provider (higher values = higher priority)
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// Gets the menu locations that this provider supports
        /// </summary>
        IEnumerable<string> SupportedLocations { get; }

        /// <summary>
        /// Gets menu items for the specified location
        /// </summary>
        /// <param name="location">The menu location to get items for</param>
        /// <returns>Menu items for the specified location</returns>
        IEnumerable<MenuItem> GetMenuItems(string location);

        /// <summary>
        /// Gets menu items for the specified location asynchronously
        /// </summary>
        /// <param name="location">The menu location to get items for</param>
        /// <param name="user">The current user context</param>
        /// <returns>Menu items for the specified location</returns>
        Task<IEnumerable<MenuItem>> GetMenuItemsAsync(string location, ClaimsPrincipal? user = null);

        /// <summary>
        /// Gets all menus provided by this provider
        /// </summary>
        /// <returns>All menus from this provider</returns>
        IEnumerable<Menu> GetMenus();

        /// <summary>
        /// Gets all menus provided by this provider asynchronously
        /// </summary>
        /// <param name="user">The current user context</param>
        /// <returns>All menus from this provider</returns>
        Task<IEnumerable<Menu>> GetMenusAsync(ClaimsPrincipal? user = null);

        /// <summary>
        /// Determines whether this provider supports the specified location
        /// </summary>
        /// <param name="location">The menu location to check</param>
        /// <returns>True if the location is supported, false otherwise</returns>
        bool SupportsLocation(string location);

        /// <summary>
        /// Gets the menu for a specific location
        /// </summary>
        /// <param name="location">The menu location</param>
        /// <returns>The menu for the specified location, or null if not found</returns>
        Menu? GetMenu(string location);

        /// <summary>
        /// Gets the menu for a specific location asynchronously
        /// </summary>
        /// <param name="location">The menu location</param>
        /// <param name="user">The current user context</param>
        /// <returns>The menu for the specified location, or null if not found</returns>
        Task<Menu?> GetMenuAsync(string location, ClaimsPrincipal? user = null);
    }

    /// <summary>
    /// Base implementation of IMenuProvider that provides common functionality
    /// </summary>
    public abstract class MenuProviderBase : IMenuProvider
    {
        /// <summary>
        /// Gets the unique identifier for this menu provider
        /// </summary>
        public abstract string ProviderId { get; }

        /// <summary>
        /// Gets the priority of this menu provider (higher values = higher priority)
        /// </summary>
        public virtual int Priority => 0;

        /// <summary>
        /// Gets the menu locations that this provider supports
        /// </summary>
        public abstract IEnumerable<string> SupportedLocations { get; }

        /// <summary>
        /// Gets menu items for the specified location
        /// </summary>
        /// <param name="location">The menu location to get items for</param>
        /// <returns>Menu items for the specified location</returns>
        public virtual IEnumerable<MenuItem> GetMenuItems(string location)
        {
            return GetMenuItemsAsync(location).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets menu items for the specified location asynchronously
        /// </summary>
        /// <param name="location">The menu location to get items for</param>
        /// <param name="user">The current user context</param>
        /// <returns>Menu items for the specified location</returns>
        public abstract Task<IEnumerable<MenuItem>> GetMenuItemsAsync(string location, ClaimsPrincipal? user = null);

        /// <summary>
        /// Gets all menus provided by this provider
        /// </summary>
        /// <returns>All menus from this provider</returns>
        public virtual IEnumerable<Menu> GetMenus()
        {
            return GetMenusAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets all menus provided by this provider asynchronously
        /// </summary>
        /// <param name="user">The current user context</param>
        /// <returns>All menus from this provider</returns>
        public virtual async Task<IEnumerable<Menu>> GetMenusAsync(ClaimsPrincipal? user = null)
        {
            var menus = new List<Menu>();

            foreach (var location in SupportedLocations)
            {
                var menuItems = await GetMenuItemsAsync(location, user);
                if (menuItems.Any())
                {
                    var menu = new Menu(location, location)
                    {
                        ProviderId = ProviderId,
                        Priority = Priority,
                        Items = menuItems.ToList()
                    };
                    menus.Add(menu);
                }
            }

            return menus;
        }

        /// <summary>
        /// Determines whether this provider supports the specified location
        /// </summary>
        /// <param name="location">The menu location to check</param>
        /// <returns>True if the location is supported, false otherwise</returns>
        public virtual bool SupportsLocation(string location)
        {
            return SupportedLocations.Contains(location, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets the menu for a specific location
        /// </summary>
        /// <param name="location">The menu location</param>
        /// <returns>The menu for the specified location, or null if not found</returns>
        public virtual Menu? GetMenu(string location)
        {
            return GetMenuAsync(location).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the menu for a specific location asynchronously
        /// </summary>
        /// <param name="location">The menu location</param>
        /// <param name="user">The current user context</param>
        /// <returns>The menu for the specified location, or null if not found</returns>
        public virtual async Task<Menu?> GetMenuAsync(string location, ClaimsPrincipal? user = null)
        {
            if (!SupportsLocation(location))
                return null;

            var menuItems = await GetMenuItemsAsync(location, user);
            if (!menuItems.Any())
                return null;

            return new Menu(location, location)
            {
                ProviderId = ProviderId,
                Priority = Priority,
                Items = menuItems.ToList()
            };
        }
    }
}