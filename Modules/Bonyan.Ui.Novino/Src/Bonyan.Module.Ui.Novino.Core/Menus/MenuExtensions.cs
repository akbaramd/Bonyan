using Bonyan.Modularity;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace Menus
{
    /// <summary>
    /// Extension methods for menu configuration and access
    /// </summary>
    public static class MenuExtensions
    {
        /// <summary>
        /// Configures menu services in the configuration context
        /// </summary>
        /// <param name="context">The configuration context</param>
        /// <param name="configure">Optional action to configure menu options</param>
        /// <returns>The configuration context for chaining</returns>
        public static BonConfigurationContext ConfigureMenus(this BonConfigurationContext context, Action<MenuConfiguration>? configure = null)
        {
            context.Services.AddSingleton<IMenuManager, MenuManager>();
            
            var menuConfig = new MenuConfiguration(context.Services);
            configure?.Invoke(menuConfig);
            
            return context;
        }

        /// <summary>
        /// Adds a menu location to the service collection
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="id">The menu location ID</param>
        /// <param name="displayName">The display name</param>
        /// <param name="description">The description</param>
        /// <param name="configure">Optional action to configure the menu location</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddMenuLocation(this IServiceCollection services, string id, string displayName, string description = "", Action<MenuLocation>? configure = null)
        {
            var location = new MenuLocation(id, displayName, description);
            configure?.Invoke(location);
            
            services.AddSingleton(location);
            return services;
        }

        /// <summary>
        /// Adds a menu provider to the service collection
        /// </summary>
        /// <typeparam name="T">The menu provider type</typeparam>
        /// <param name="services">The service collection</param>
        /// <param name="lifetime">The service lifetime</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddMenuProvider<T>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where T : class, IMenuProvider
        {
            services.Add(new ServiceDescriptor(typeof(IMenuProvider), typeof(T), lifetime));
            return services;
        }

        /// <summary>
        /// Adds a menu provider instance to the service collection
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="provider">The menu provider instance</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddMenuProvider(this IServiceCollection services, IMenuProvider provider)
        {
            services.AddSingleton<IMenuProvider>(provider);
            return services;
        }

        /// <summary>
        /// Gets the menu manager from the service provider
        /// </summary>
        /// <param name="serviceProvider">The service provider</param>
        /// <returns>The menu manager instance</returns>
        public static IMenuManager GetMenuManager(this IServiceProvider serviceProvider)
        {
            return serviceProvider.GetRequiredService<IMenuManager>();
        }

        /// <summary>
        /// Gets menu items for a specific location
        /// </summary>
        /// <param name="serviceProvider">The service provider</param>
        /// <param name="location">The menu location</param>
        /// <param name="user">The current user context</param>
        /// <returns>Menu items for the specified location</returns>
        public static IEnumerable<MenuItem> GetMenuItems(this IServiceProvider serviceProvider, string location, ClaimsPrincipal? user = null)
        {
            var menuManager = serviceProvider.GetMenuManager();
            return menuManager.GetMenuItems(location, user);
        }

        /// <summary>
        /// Gets menu items for a specific location asynchronously
        /// </summary>
        /// <param name="serviceProvider">The service provider</param>
        /// <param name="location">The menu location</param>
        /// <param name="user">The current user context</param>
        /// <returns>Menu items for the specified location</returns>
        public static Task<IEnumerable<MenuItem>> GetMenuItemsAsync(this IServiceProvider serviceProvider, string location, ClaimsPrincipal? user = null)
        {
            var menuManager = serviceProvider.GetMenuManager();
            return menuManager.GetMenuItemsAsync(location, user);
        }

        /// <summary>
        /// Gets all available menu locations
        /// </summary>
        /// <param name="serviceProvider">The service provider</param>
        /// <returns>All registered menu locations</returns>
        public static IEnumerable<MenuLocation> GetMenuLocations(this IServiceProvider serviceProvider)
        {
            var menuManager = serviceProvider.GetMenuManager();
            return menuManager.MenuLocations;
        }

        /// <summary>
        /// Gets all registered menu providers
        /// </summary>
        /// <param name="serviceProvider">The service provider</param>
        /// <returns>All registered menu providers</returns>
        public static IEnumerable<IMenuProvider> GetMenuProviders(this IServiceProvider serviceProvider)
        {
            var menuManager = serviceProvider.GetMenuManager();
            return menuManager.MenuProviders;
        }
    }

    /// <summary>
    /// Configuration class for menu setup
    /// </summary>
    public class MenuConfiguration
    {
        private readonly IServiceCollection _services;

        public MenuConfiguration(IServiceCollection services)
        {
            _services = services;
        }

        /// <summary>
        /// Adds a menu location
        /// </summary>
        /// <param name="id">The menu location ID</param>
        /// <param name="displayName">The display name</param>
        /// <param name="description">The description</param>
        /// <param name="configure">Optional action to configure the menu location</param>
        /// <returns>The menu configuration for chaining</returns>
        public MenuConfiguration AddLocation(string id, string displayName, string description = "", Action<MenuLocation>? configure = null)
        {
            _services.AddMenuLocation(id, displayName, description, configure);
            return this;
        }

        /// <summary>
        /// Adds a menu provider
        /// </summary>
        /// <typeparam name="T">The menu provider type</typeparam>
        /// <param name="lifetime">The service lifetime</param>
        /// <returns>The menu configuration for chaining</returns>
        public MenuConfiguration AddProvider<T>(ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where T : class, IMenuProvider
        {
            _services.AddMenuProvider<T>(lifetime);
            return this;
        }

        /// <summary>
        /// Adds a menu provider instance
        /// </summary>
        /// <param name="provider">The menu provider instance</param>
        /// <returns>The menu configuration for chaining</returns>
        public MenuConfiguration AddProvider(IMenuProvider provider)
        {
            _services.AddMenuProvider(provider);
            return this;
        }

        /// <summary>
        /// Configures common menu locations
        /// </summary>
        /// <returns>The menu configuration for chaining</returns>
        public MenuConfiguration AddCommonLocations()
        {
            AddLocation("main-navigation", "Main Navigation", "Primary navigation menu");
            AddLocation("sidebar", "Sidebar", "Sidebar navigation menu");
            AddLocation("footer", "Footer", "Footer navigation menu");
            AddLocation("admin", "Admin Menu", "Administrative menu");
            AddLocation("user-profile", "User Profile", "User profile menu");
            AddLocation("breadcrumb", "Breadcrumb", "Breadcrumb navigation");
            AddLocation("toolbar", "Toolbar", "Toolbar menu");
            AddLocation("context-menu", "Context Menu", "Context-sensitive menu");
            
            return this;
        }

        /// <summary>
        /// Configures a location with specific settings
        /// </summary>
        /// <param name="id">The menu location ID</param>
        /// <param name="displayName">The display name</param>
        /// <param name="description">The description</param>
        /// <param name="maxDepth">Maximum menu depth</param>
        /// <param name="supportsNesting">Whether nesting is supported</param>
        /// <param name="cssClass">CSS class for the location</param>
        /// <returns>The menu configuration for chaining</returns>
        public MenuConfiguration AddLocation(string id, string displayName, string description, int maxDepth, bool supportsNesting, string cssClass = "")
        {
            _services.AddMenuLocation(id, displayName, description, location =>
            {
                location.MaxDepth = maxDepth;
                location.SupportsNesting = supportsNesting;
                location.CssClass = cssClass;
            });
            return this;
        }
    }
} 