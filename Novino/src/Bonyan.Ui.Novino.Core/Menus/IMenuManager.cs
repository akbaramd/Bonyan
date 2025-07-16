using System.Security.Claims;
using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Users;

namespace Bonyan.Novino.Core.Menus
{
    /// <summary>
    /// Defines a contract for the menu manager that orchestrates menu providers and locations
    /// </summary>
    public interface IMenuManager<TUser, TRole> where TUser : BonIdentityUser<TUser, TRole> where TRole : BonIdentityRole<TRole>
    {
        /// <summary>
        /// Gets all registered menu providers
        /// </summary>
        IEnumerable<IMenuProvider> MenuProviders { get; }

        /// <summary>
        /// Gets all registered menu locations
        /// </summary>
        IEnumerable<MenuLocation> MenuLocations { get; }

        /// <summary>
        /// Gets the menu for a specific location
        /// </summary>
        /// <param name="location">The menu location</param>
        /// <param name="user">The current user context</param>
        /// <returns>The combined menu for the specified location</returns>
        Task<Menu?> GetMenuAsync(string location, ClaimsPrincipal? user = null);

        /// <summary>
        /// Gets the menu for a specific location synchronously
        /// </summary>
        /// <param name="location">The menu location</param>
        /// <param name="user">The current user context</param>
        /// <returns>The combined menu for the specified location</returns>
        Menu? GetMenu(string location, ClaimsPrincipal? user = null);

        /// <summary>
        /// Gets menu items for a specific location
        /// </summary>
        /// <param name="location">The menu location</param>
        /// <param name="user">The current user context</param>
        /// <returns>Menu items for the specified location</returns>
        Task<IEnumerable<MenuItem>> GetMenuItemsAsync(string location, ClaimsPrincipal? user = null);

        /// <summary>
        /// Gets menu items for a specific location synchronously
        /// </summary>
        /// <param name="location">The menu location</param>
        /// <param name="user">The current user context</param>
        /// <returns>Menu items for the specified location</returns>
        IEnumerable<MenuItem> GetMenuItems(string location, ClaimsPrincipal? user = null);

        /// <summary>
        /// Gets all menus from all providers
        /// </summary>
        /// <param name="user">The current user context</param>
        /// <returns>All menus from all providers</returns>
        Task<IEnumerable<Menu>> GetAllMenusAsync(ClaimsPrincipal? user = null);

        /// <summary>
        /// Gets all menus from all providers synchronously
        /// </summary>
        /// <param name="user">The current user context</param>
        /// <returns>All menus from all providers</returns>
        IEnumerable<Menu> GetAllMenus(ClaimsPrincipal? user = null);

        /// <summary>
        /// Registers a menu provider
        /// </summary>
        /// <param name="provider">The menu provider to register</param>
        void RegisterProvider(IMenuProvider provider);

        /// <summary>
        /// Unregisters a menu provider
        /// </summary>
        /// <param name="provider">The menu provider to unregister</param>
        void UnregisterProvider(IMenuProvider provider);

        /// <summary>
        /// Registers a menu location
        /// </summary>
        /// <param name="location">The menu location to register</param>
        void RegisterLocation(MenuLocation location);

        /// <summary>
        /// Unregisters a menu location
        /// </summary>
        /// <param name="locationId">The ID of the menu location to unregister</param>
        void UnregisterLocation(string locationId);

        /// <summary>
        /// Gets a menu location by ID
        /// </summary>
        /// <param name="locationId">The menu location ID</param>
        /// <returns>The menu location if found, null otherwise</returns>
        MenuLocation? GetLocation(string locationId);

        /// <summary>
        /// Checks if a menu location is registered
        /// </summary>
        /// <param name="locationId">The menu location ID</param>
        /// <returns>True if the location is registered, false otherwise</returns>
        bool IsLocationRegistered(string locationId);

        /// <summary>
        /// Gets providers that support a specific location
        /// </summary>
        /// <param name="location">The menu location</param>
        /// <returns>Providers that support the specified location</returns>
        IEnumerable<IMenuProvider> GetProvidersForLocation(string location);
    }
} 