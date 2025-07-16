using System.Security.Claims;
using Bonyan.Modularity;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Novino.Core.Assets
{
    /// <summary>
    /// Extension methods for asset configuration and access
    /// </summary>
    public static class AssetExtensions
    {
        /// <summary>
        /// Configures asset services in the configuration context
        /// </summary>
        /// <param name="context">The configuration context</param>
        /// <param name="configure">Optional action to configure asset options</param>
        /// <returns>The configuration context for chaining</returns>
        public static BonConfigurationContext ConfigureAssets(this BonConfigurationContext context, Action<AssetConfiguration>? configure = null)
        {
            context.Services.AddSingleton<IAssetManager, AssetManager>();
            
            // Register tag helpers explicitly
            context.Services.AddTransient<Assets.TagHelpers.AssetTagHelper>();
            context.Services.AddTransient<Assets.TagHelpers.CssAssetTagHelper>();
            context.Services.AddTransient<Assets.TagHelpers.JavaScriptAssetTagHelper>();
            context.Services.AddTransient<Assets.TagHelpers.InlineCssAssetTagHelper>();
            context.Services.AddTransient<Assets.TagHelpers.InlineJavaScriptAssetTagHelper>();
            
            var assetConfig = new AssetConfiguration(context.Services);
            configure?.Invoke(assetConfig);
            
            return context;
        }

        /// <summary>
        /// Adds an asset provider to the service collection
        /// </summary>
        /// <typeparam name="T">The asset provider type</typeparam>
        /// <param name="services">The service collection</param>
        /// <param name="lifetime">The service lifetime</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddAssetProvider<T>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where T : class, IAssetProvider
        {
            services.Add(new ServiceDescriptor(typeof(IAssetProvider), typeof(T), lifetime));
            return services;
        }

        /// <summary>
        /// Adds an asset provider instance to the service collection
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="provider">The asset provider instance</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddAssetProvider(this IServiceCollection services, IAssetProvider provider)
        {
            services.AddSingleton<IAssetProvider>(provider);
            return services;
        }

        /// <summary>
        /// Gets the asset manager from the service provider
        /// </summary>
        /// <param name="serviceProvider">The service provider</param>
        /// <returns>The asset manager instance</returns>
        public static IAssetManager GetAssetManager(this IServiceProvider serviceProvider)
        {
            return serviceProvider.GetRequiredService<IAssetManager>();
        }

        /// <summary>
        /// Gets assets for a specific location
        /// </summary>
        /// <param name="serviceProvider">The service provider</param>
        /// <param name="location">The asset location</param>
        /// <param name="user">The current user context</param>
        /// <returns>Assets for the specified location</returns>
        public static IEnumerable<Asset> GetAssets(this IServiceProvider serviceProvider, AssetLocation location, ClaimsPrincipal? user = null)
        {
            var assetManager = serviceProvider.GetAssetManager();
            return assetManager.GetAssets(location, user);
        }

        /// <summary>
        /// Gets assets for a specific location asynchronously
        /// </summary>
        /// <param name="serviceProvider">The service provider</param>
        /// <param name="location">The asset location</param>
        /// <param name="user">The current user context</param>
        /// <returns>Assets for the specified location</returns>
        public static Task<IEnumerable<Asset>> GetAssetsAsync(this IServiceProvider serviceProvider, AssetLocation location, ClaimsPrincipal? user = null)
        {
            var assetManager = serviceProvider.GetAssetManager();
            return assetManager.GetAssetsAsync(location, user);
        }

        /// <summary>
        /// Gets CSS assets for a specific location
        /// </summary>
        /// <param name="serviceProvider">The service provider</param>
        /// <param name="location">The asset location</param>
        /// <param name="user">The current user context</param>
        /// <returns>CSS assets for the specified location</returns>
        public static IEnumerable<Asset> GetCssAssets(this IServiceProvider serviceProvider, AssetLocation location = AssetLocation.Head, ClaimsPrincipal? user = null)
        {
            var assetManager = serviceProvider.GetAssetManager();
            return assetManager.GetAssetsByType(location, AssetType.Css, user);
        }

        /// <summary>
        /// Gets JavaScript assets for a specific location
        /// </summary>
        /// <param name="serviceProvider">The service provider</param>
        /// <param name="location">The asset location</param>
        /// <param name="user">The current user context</param>
        /// <returns>JavaScript assets for the specified location</returns>
        public static IEnumerable<Asset> GetJavaScriptAssets(this IServiceProvider serviceProvider, AssetLocation location = AssetLocation.Footer, ClaimsPrincipal? user = null)
        {
            var assetManager = serviceProvider.GetAssetManager();
            return assetManager.GetAssetsByType(location, AssetType.JavaScript, user);
        }

        /// <summary>
        /// Renders assets as HTML for a specific location
        /// </summary>
        /// <param name="serviceProvider">The service provider</param>
        /// <param name="location">The asset location</param>
        /// <param name="user">The current user context</param>
        /// <returns>HTML string containing the assets</returns>
        public static string RenderAssets(this IServiceProvider serviceProvider, AssetLocation location, ClaimsPrincipal? user = null)
        {
            var assetManager = serviceProvider.GetAssetManager();
            return assetManager.RenderAssets(location, user);
        }

        /// <summary>
        /// Renders assets as HTML for a specific location asynchronously
        /// </summary>
        /// <param name="serviceProvider">The service provider</param>
        /// <param name="location">The asset location</param>
        /// <param name="user">The current user context</param>
        /// <returns>HTML string containing the assets</returns>
        public static Task<string> RenderAssetsAsync(this IServiceProvider serviceProvider, AssetLocation location, ClaimsPrincipal? user = null)
        {
            var assetManager = serviceProvider.GetAssetManager();
            return assetManager.RenderAssetsAsync(location, user);
        }

        /// <summary>
        /// Adds a CSS file asset
        /// </summary>
        /// <param name="serviceProvider">The service provider</param>
        /// <param name="name">Asset name</param>
        /// <param name="href">CSS file path or URL</param>
        /// <param name="location">Where to render the asset</param>
        /// <param name="priority">Loading priority</param>
        public static void AddCssAsset(this IServiceProvider serviceProvider, string name, string href, AssetLocation location = AssetLocation.Head, int priority = 100)
        {
            var assetManager = serviceProvider.GetAssetManager();
            var asset = new CssAsset(name, href) { Priority = priority };
            assetManager.AddAsset(location, asset);
        }

        /// <summary>
        /// Adds a JavaScript file asset
        /// </summary>
        /// <param name="serviceProvider">The service provider</param>
        /// <param name="name">Asset name</param>
        /// <param name="src">JavaScript file path or URL</param>
        /// <param name="location">Where to render the asset</param>
        /// <param name="priority">Loading priority</param>
        /// <param name="loadingStrategy">Loading strategy (async, defer, etc.)</param>
        public static void AddJavaScriptAsset(this IServiceProvider serviceProvider, string name, string src, AssetLocation location = AssetLocation.Footer, int priority = 100, AssetLoadingStrategy loadingStrategy = AssetLoadingStrategy.Normal)
        {
            var assetManager = serviceProvider.GetAssetManager();
            var asset = new JavaScriptAsset(name, src) 
            { 
                Priority = priority, 
                LoadingStrategy = loadingStrategy 
            };
            assetManager.AddAsset(location, asset);
        }

        /// <summary>
        /// Adds inline CSS
        /// </summary>
        /// <param name="serviceProvider">The service provider</param>
        /// <param name="name">Asset name</param>
        /// <param name="content">CSS content</param>
        /// <param name="location">Where to render the asset</param>
        /// <param name="priority">Loading priority</param>
        public static void AddInlineCss(this IServiceProvider serviceProvider, string name, string content, AssetLocation location = AssetLocation.InlineHead, int priority = 100)
        {
            var assetManager = serviceProvider.GetAssetManager();
            var asset = new InlineCssAsset(name, content) { Priority = priority };
            assetManager.AddAsset(location, asset);
        }

        /// <summary>
        /// Adds inline JavaScript
        /// </summary>
        /// <param name="serviceProvider">The service provider</param>
        /// <param name="name">Asset name</param>
        /// <param name="content">JavaScript content</param>
        /// <param name="location">Where to render the asset</param>
        /// <param name="priority">Loading priority</param>
        public static void AddInlineJavaScript(this IServiceProvider serviceProvider, string name, string content, AssetLocation location = AssetLocation.InlineFooter, int priority = 100)
        {
            var assetManager = serviceProvider.GetAssetManager();
            var asset = new InlineJavaScriptAsset(name, content) { Priority = priority };
            assetManager.AddAsset(location, asset);
        }
    }

    /// <summary>
    /// Configuration class for asset setup
    /// </summary>
    public class AssetConfiguration
    {
        private readonly IServiceCollection _services;

        public AssetConfiguration(IServiceCollection services)
        {
            _services = services;
        }

        /// <summary>
        /// Adds an asset provider
        /// </summary>
        /// <typeparam name="T">The asset provider type</typeparam>
        /// <param name="lifetime">The service lifetime</param>
        /// <returns>The asset configuration for chaining</returns>
        public AssetConfiguration AddProvider<T>(ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where T : class, IAssetProvider
        {
            _services.AddAssetProvider<T>(lifetime);
            return this;
        }

        /// <summary>
        /// Adds an asset provider instance
        /// </summary>
        /// <param name="provider">The asset provider instance</param>
        /// <returns>The asset configuration for chaining</returns>
        public AssetConfiguration AddProvider(IAssetProvider provider)
        {
            _services.AddAssetProvider(provider);
            return this;
        }

        /// <summary>
        /// Configures common framework assets (jQuery, Bootstrap, etc.)
        /// </summary>
        /// <returns>The asset configuration for chaining</returns>
        public AssetConfiguration AddCommonAssets()
        {
            AddProvider<CommonFrameworkAssetProvider>();
            return this;
        }

        /// <summary>
        /// Adds support for CDN assets with fallbacks
        /// </summary>
        /// <returns>The asset configuration for chaining</returns>
        public AssetConfiguration AddCdnSupport()
        {
            // This could be extended to add CDN providers
            return this;
        }
    }

    /// <summary>
    /// Common framework asset provider for jQuery, Bootstrap, etc.
    /// </summary>
    public class CommonFrameworkAssetProvider : AssetProviderBase
    {
        public override string ProviderId => "common-framework-assets";

        public override int Priority => 1000; // High priority for framework assets

        public override IEnumerable<AssetLocation> SupportedLocations => new[]
        {
            AssetLocation.Head,
            AssetLocation.Footer
        };

        public override async Task<IEnumerable<Asset>> GetAssetsAsync(AssetLocation location, ClaimsPrincipal? user = null)
        {
            await Task.Delay(1); // Simulate async operation

            var assets = new List<Asset>();

            switch (location)
            {
                case AssetLocation.Head:
                    // Add Bootstrap CSS
                    assets.Add(CreateCssAsset("bootstrap", "~/lib/bootstrap/dist/css/bootstrap.min.css", 10));
                    break;

                case AssetLocation.Footer:
                    // Add jQuery first
                    assets.Add(CreateJavaScriptAsset("jquery", "~/lib/jquery/dist/jquery.min.js", 1));
                    
                    // Add Bootstrap JS (depends on jQuery)
                    var bootstrapJs = CreateJavaScriptAsset("bootstrap", "~/lib/bootstrap/dist/js/bootstrap.bundle.min.js", 10);
                    bootstrapJs.Dependencies.Add("jquery");
                    assets.Add(bootstrapJs);
                    break;
            }

            return assets;
        }
    }
} 