using System.Collections.Concurrent;
using System.Security.Claims;
using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Users;
using Microsoft.Extensions.Logging;
using Bonyan.IdentityManagement.Permissions;
using Bonyan.User;
using Bonyan.UserManagement.Domain.Users.ValueObjects;

namespace Menus
{
    /// <summary>
    /// Default implementation of IMenuManager that orchestrates menu providers and locations
    /// </summary>
    public class MenuManager<TUser,TRole> : IMenuManager<TUser,TRole> where TUser : BonIdentityUser<TUser,TRole> where TRole : BonIdentityRole<TRole>
    {
        private readonly ILogger<MenuManager<TUser,TRole>> _logger;
        private readonly IBonPermissionManager<TUser,TRole>? _permissionManager;
        private readonly IBonCurrentUser? _currentUser;
        private readonly ConcurrentDictionary<string, IMenuProvider> _providers = new();
        private readonly ConcurrentDictionary<string, MenuLocation> _locations = new();

        /// <summary>
        /// Gets all registered menu providers
        /// </summary>
        public IEnumerable<IMenuProvider> MenuProviders => _providers.Values;

        /// <summary>
        /// Gets all registered menu locations
        /// </summary>
        public IEnumerable<MenuLocation> MenuLocations => _locations.Values;

        public MenuManager(
            ILogger<MenuManager<TUser,TRole>> logger, 
            IBonPermissionManager<TUser,TRole>? permissionManager = null,
            IBonCurrentUser? currentUser = null)
        {
            _logger = logger;
            _permissionManager = permissionManager;
            _currentUser = currentUser;
        }

        /// <summary>
        /// Gets the menu for a specific location
        /// </summary>
        /// <param name="location">The menu location</param>
        /// <param name="user">The current user context</param>
        /// <returns>The combined menu for the specified location</returns>
        public async Task<Menu?> GetMenuAsync(string location, ClaimsPrincipal? user = null)
        {
            if (string.IsNullOrEmpty(location))
                return null;

            try
            {
                var menuItems = await GetMenuItemsAsync(location, user);
                
                if (!menuItems.Any())
                    return null;

                var menu = new Menu(location, location)
                {
                    Items = menuItems.ToList(),
                    IsEnabled = true,
                    ProviderId = "MenuManager"
                };

                // Check menu-level authorities
                if (!await CheckMenuAuthorityAsync(menu, user))
                    return null;

                return menu;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting menu for location '{Location}'", location);
                return null;
            }
        }

        /// <summary>
        /// Gets the menu for a specific location synchronously
        /// </summary>
        /// <param name="location">The menu location</param>
        /// <param name="user">The current user context</param>
        /// <returns>The combined menu for the specified location</returns>
        public Menu? GetMenu(string location, ClaimsPrincipal? user = null)
        {
            return GetMenuAsync(location, user).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets menu items for a specific location
        /// </summary>
        /// <param name="location">The menu location</param>
        /// <param name="user">The current user context</param>
        /// <returns>Menu items for the specified location</returns>
        public async Task<IEnumerable<MenuItem>> GetMenuItemsAsync(string location, ClaimsPrincipal? user = null)
        {
            if (string.IsNullOrEmpty(location))
                return Enumerable.Empty<MenuItem>();

            try
            {
                var menuLocation = GetLocation(location);
                if (menuLocation != null && !menuLocation.IsActive)
                {
                    _logger.LogDebug("Menu location '{Location}' is not active", location);
                    return Enumerable.Empty<MenuItem>();
                }

                var providers = GetProvidersForLocation(location);
                var allMenuItems = new List<MenuItem>();

                // Get menu items from all providers that support this location
                foreach (var provider in providers.OrderByDescending(p => p.Priority))
                {
                    try
                    {
                        var items = await provider.GetMenuItemsAsync(location, user);
                        if (items.Any())
                        {
                            allMenuItems.AddRange(items);
                            _logger.LogDebug("Provider '{ProviderId}' contributed {Count} menu items for location '{Location}'", 
                                provider.ProviderId, items.Count(), location);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error getting menu items from provider '{ProviderId}' for location '{Location}'", 
                            provider.ProviderId, location);
                    }
                }

                // Filter visible items and sort by order
                var visibleItems = new List<MenuItem>();
                
                foreach (var item in allMenuItems)
                {
                    if (await CheckMenuItemVisibilityAsync(item, user))
                    {
                        visibleItems.Add(item);
                    }
                }
                
                visibleItems = visibleItems.OrderBy(item => item.Order).ToList();

                // Apply menu location constraints
                if (menuLocation != null)
                {
                    visibleItems = ApplyLocationConstraints(visibleItems, menuLocation);
                }

                return visibleItems;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting menu items for location '{Location}'", location);
                return Enumerable.Empty<MenuItem>();
            }
        }

        /// <summary>
        /// Gets menu items for a specific location synchronously
        /// </summary>
        /// <param name="location">The menu location</param>
        /// <param name="user">The current user context</param>
        /// <returns>Menu items for the specified location</returns>
        public IEnumerable<MenuItem> GetMenuItems(string location, ClaimsPrincipal? user = null)
        {
            return GetMenuItemsAsync(location, user).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets all menus from all providers
        /// </summary>
        /// <param name="user">The current user context</param>
        /// <returns>All menus from all providers</returns>
        public async Task<IEnumerable<Menu>> GetAllMenusAsync(ClaimsPrincipal? user = null)
        {
            var allMenus = new List<Menu>();

            try
            {
                // Get menus from all providers
                foreach (var provider in MenuProviders)
                {
                    try
                    {
                        var menus = await provider.GetMenusAsync(user);
                        
                        // Check menu-level authorities for each menu
                        foreach (var menu in menus)
                        {
                            if (await CheckMenuAuthorityAsync(menu, user))
                            {
                                allMenus.Add(menu);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error getting menus from provider '{ProviderId}'", provider.ProviderId);
                    }
                }

                // Also create menus for registered locations that don't have explicit menus
                foreach (var location in MenuLocations.Where(l => l.IsActive))
                {
                    if (!allMenus.Any(m => string.Equals(m.Location, location.Id, StringComparison.OrdinalIgnoreCase)))
                    {
                        var menu = await GetMenuAsync(location.Id, user);
                        if (menu != null && await CheckMenuAuthorityAsync(menu, user))
                        {
                            allMenus.Add(menu);
                        }
                    }
                }

                return allMenus;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all menus");
                return Enumerable.Empty<Menu>();
            }
        }

        /// <summary>
        /// Gets all menus from all providers synchronously
        /// </summary>
        /// <param name="user">The current user context</param>
        /// <returns>All menus from all providers</returns>
        public IEnumerable<Menu> GetAllMenus(ClaimsPrincipal? user = null)
        {
            return GetAllMenusAsync(user).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the menu for a specific location using the current user context
        /// </summary>
        /// <param name="location">The menu location</param>
        /// <returns>The combined menu for the specified location</returns>
        public async Task<Menu?> GetMenuForCurrentUserAsync(string location)
        {
            if (_currentUser?.IsAuthenticated != true)
                return await GetMenuAsync(location, null);
            
            // Create a minimal ClaimsPrincipal for compatibility with existing checks
            var claims = new List<Claim>();
            if (_currentUser.UserName != null)
                claims.Add(new Claim(ClaimTypes.Name, _currentUser.UserName));
            if (_currentUser.Id != null)
                claims.Add(new Claim(ClaimTypes.NameIdentifier, _currentUser.Id.Value.ToString()));
            
            foreach (var role in _currentUser.Roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var identity = new ClaimsIdentity(claims, "current-user");
            var principal = new ClaimsPrincipal(identity);
            
            return await GetMenuAsync(location, principal);
        }

        /// <summary>
        /// Gets menu items for a specific location using the current user context
        /// </summary>
        /// <param name="location">The menu location</param>
        /// <returns>Menu items for the specified location</returns>
        public async Task<IEnumerable<MenuItem>> GetMenuItemsForCurrentUserAsync(string location)
        {
            if (_currentUser?.IsAuthenticated != true)
                return await GetMenuItemsAsync(location, null);
            
            // Create a minimal ClaimsPrincipal for compatibility with existing checks
            var claims = new List<Claim>();
            if (_currentUser.UserName != null)
                claims.Add(new Claim(ClaimTypes.Name, _currentUser.UserName));
            if (_currentUser.Id != null)
                claims.Add(new Claim(ClaimTypes.NameIdentifier, _currentUser.Id.Value.ToString()));
            
            foreach (var role in _currentUser.Roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var identity = new ClaimsIdentity(claims, "current-user");
            var principal = new ClaimsPrincipal(identity);
            
            return await GetMenuItemsAsync(location, principal);
        }

        /// <summary>
        /// Registers a menu provider
        /// </summary>
        /// <param name="provider">The menu provider to register</param>
        public void RegisterProvider(IMenuProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            _providers.TryAdd(provider.ProviderId, provider);
            _logger.LogInformation("Registered menu provider '{ProviderId}' supporting locations: {Locations}", 
                provider.ProviderId, string.Join(", ", provider.SupportedLocations));
        }

        /// <summary>
        /// Unregisters a menu provider
        /// </summary>
        /// <param name="provider">The menu provider to unregister</param>
        public void UnregisterProvider(IMenuProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            if (_providers.TryRemove(provider.ProviderId, out _))
            {
                _logger.LogInformation("Unregistered menu provider '{ProviderId}'", provider.ProviderId);
            }
        }

        /// <summary>
        /// Registers a menu location
        /// </summary>
        /// <param name="location">The menu location to register</param>
        public void RegisterLocation(MenuLocation location)
        {
            if (location == null)
                throw new ArgumentNullException(nameof(location));

            _locations.TryAdd(location.Id, location);
            _logger.LogInformation("Registered menu location '{LocationId}' - {DisplayName}", 
                location.Id, location.DisplayName);
        }

        /// <summary>
        /// Unregisters a menu location
        /// </summary>
        /// <param name="locationId">The ID of the menu location to unregister</param>
        public void UnregisterLocation(string locationId)
        {
            if (string.IsNullOrEmpty(locationId))
                throw new ArgumentNullException(nameof(locationId));

            if (_locations.TryRemove(locationId, out _))
            {
                _logger.LogInformation("Unregistered menu location '{LocationId}'", locationId);
            }
        }

        /// <summary>
        /// Gets a menu location by ID
        /// </summary>
        /// <param name="locationId">The menu location ID</param>
        /// <returns>The menu location if found, null otherwise</returns>
        public MenuLocation? GetLocation(string locationId)
        {
            if (string.IsNullOrEmpty(locationId))
                return null;

            _locations.TryGetValue(locationId, out var location);
            return location;
        }

        /// <summary>
        /// Checks if a menu location is registered
        /// </summary>
        /// <param name="locationId">The menu location ID</param>
        /// <returns>True if the location is registered, false otherwise</returns>
        public bool IsLocationRegistered(string locationId)
        {
            return !string.IsNullOrEmpty(locationId) && _locations.ContainsKey(locationId);
        }

        /// <summary>
        /// Gets providers that support a specific location
        /// </summary>
        /// <param name="location">The menu location</param>
        /// <returns>Providers that support the specified location</returns>
        public IEnumerable<IMenuProvider> GetProvidersForLocation(string location)
        {
            if (string.IsNullOrEmpty(location))
                return Enumerable.Empty<IMenuProvider>();

            return MenuProviders.Where(p => p.SupportsLocation(location));
        }

        /// <summary>
        /// Applies location-specific constraints to menu items
        /// </summary>
        /// <param name="items">The menu items to filter</param>
        /// <param name="location">The menu location with constraints</param>
        /// <returns>Filtered menu items</returns>
        private List<MenuItem> ApplyLocationConstraints(List<MenuItem> items, MenuLocation location)
        {
            if (!location.SupportsNesting)
            {
                // Flatten the menu structure if nesting is not supported
                var flattenedItems = new List<MenuItem>();
                foreach (var item in items)
                {
                    flattenedItems.Add(item);
                    if (item.HasChildren)
                    {
                        flattenedItems.AddRange(item.GetAllDescendants());
                        item.Children.Clear(); // Remove children since nesting is not supported
                    }
                }
                items = flattenedItems;
            }
            else if (location.MaxDepth > 0)
            {
                // Limit the depth of menu items
                items = LimitMenuDepth(items, location.MaxDepth);
            }

            return items;
        }

        /// <summary>
        /// Limits the depth of menu items to the specified maximum depth
        /// </summary>
        /// <param name="items">The menu items to limit</param>
        /// <param name="maxDepth">The maximum allowed depth</param>
        /// <returns>Menu items with limited depth</returns>
        private List<MenuItem> LimitMenuDepth(List<MenuItem> items, int maxDepth)
        {
            var result = new List<MenuItem>();

            foreach (var item in items)
            {
                var limitedItem = CloneMenuItemWithLimitedDepth(item, maxDepth, 0);
                result.Add(limitedItem);
            }

            return result;
        }

        /// <summary>
        /// Clones a menu item with limited depth
        /// </summary>
        /// <param name="item">The menu item to clone</param>
        /// <param name="maxDepth">The maximum allowed depth</param>
        /// <param name="currentDepth">The current depth</param>
        /// <returns>Cloned menu item with limited depth</returns>
        private MenuItem CloneMenuItemWithLimitedDepth(MenuItem item, int maxDepth, int currentDepth)
        {
            var clonedItem = new MenuItem(item.Title, item.Url, item.Icon, item.Order)
            {
                Id = item.Id,
                CssClass = item.CssClass,
                Target = item.Target,
                IsEnabled = item.IsEnabled,
                IsVisible = item.IsVisible,
                RequiredPermissions = new List<string>(item.RequiredPermissions),
                RequiredRoles = new List<string>(item.RequiredRoles),
                RequiresAuthentication = item.RequiresAuthentication,
                Authority = new List<string>(item.Authority),
                Metadata = new Dictionary<string, object>(item.Metadata),
                VisibilityCondition = item.VisibilityCondition
            };

            if (currentDepth < maxDepth && item.HasChildren)
            {
                foreach (var child in item.Children)
                {
                    var clonedChild = CloneMenuItemWithLimitedDepth(child, maxDepth, currentDepth + 1);
                    clonedItem.AddChild(clonedChild);
                }
            }

            return clonedItem;
        }

        /// <summary>
        /// Checks if a menu item is visible to the specified user including authority checks
        /// </summary>
        /// <param name="item">The menu item to check</param>
        /// <param name="user">The user to check visibility for</param>
        /// <returns>True if the item is visible to the user</returns>
        private async Task<bool> CheckMenuItemVisibilityAsync(MenuItem item, ClaimsPrincipal? user = null)
        {
            // First check basic visibility (this includes authentication, roles, and basic permissions)
            if (!item.CheckIsVisible(user))
                return false;

            // Check authorities using permission manager and current user if available
            if (item.Authority.Any() && _permissionManager != null && _currentUser?.IsAuthenticated == true)
            {
                if (!await CheckUserHasAnyAuthorityAsync(user, item.Authority))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if a user has any of the required authorities
        /// </summary>
        /// <param name="user">The user to check</param>
        /// <param name="authorities">The required authorities</param>
        /// <returns>True if the user has any of the required authorities</returns>
        private async Task<bool> CheckUserHasAnyAuthorityAsync(ClaimsPrincipal user, List<string> authorities)
        {
            if (_permissionManager == null || !authorities.Any())
                return true;

            try
            {
                // Use IBonCurrentUser to get the current user ID
                if (_currentUser?.Id == null)
                {
                    _logger.LogWarning("Current user ID is not available for user {UserName}", user.Identity?.Name);
                    return false;
                }

                // Convert Guid to BonUserId
                var userId = BonUserId.NewId(_currentUser.Id.Value);

                // Use the new HasAnyPermissionAsync method which is more efficient
                var hasPermission = await _permissionManager.HasAnyPermissionAsync(userId, authorities);
                
                _logger.LogDebug("User {UserId} ({UserName}) permission check result: {HasPermission} for authorities: {Authorities}", 
                    _currentUser.Id, _currentUser.UserName, hasPermission, string.Join(", ", authorities));
                
                return hasPermission;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking user authorities for user {UserId} ({UserName})", 
                    _currentUser?.Id, _currentUser?.UserName);
                return false;
            }
        }

        /// <summary>
        /// Checks if a menu has the required authorities for the specified user
        /// </summary>
        /// <param name="menu">The menu to check</param>
        /// <param name="user">The user to check authorities for</param>
        /// <returns>True if the user has access to the menu</returns>
        private async Task<bool> CheckMenuAuthorityAsync(Menu menu, ClaimsPrincipal? user = null)
        {
            // Check authentication requirement using IBonCurrentUser if available, otherwise fallback to ClaimsPrincipal
            if (menu.RequiresAuthentication)
            {
                var isAuthenticated = _currentUser?.IsAuthenticated ?? (user?.Identity?.IsAuthenticated == true);
                if (!isAuthenticated)
                    return false;
            }

            // Check authorities using permission manager if available
            if (menu.Authority.Any() && _permissionManager != null && user != null)
            {
                if (!await CheckUserHasAnyAuthorityAsync(user, menu.Authority))
                    return false;
            }

            return true;
        }
    }
} 