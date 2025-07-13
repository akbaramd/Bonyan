using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Assets
{
    /// <summary>
    /// Defines a contract for asset providers that can contribute assets to the application
    /// </summary>
    public interface IAssetProvider
    {
        /// <summary>
        /// Gets the unique identifier for this asset provider
        /// </summary>
        string ProviderId { get; }

        /// <summary>
        /// Gets the priority of this asset provider (higher values = higher priority)
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// Gets the asset locations that this provider supports
        /// </summary>
        IEnumerable<AssetLocation> SupportedLocations { get; }

        /// <summary>
        /// Gets assets for the specified location
        /// </summary>
        /// <param name="location">The asset location to get assets for</param>
        /// <returns>Assets for the specified location</returns>
        IEnumerable<Asset> GetAssets(AssetLocation location);

        /// <summary>
        /// Gets assets for the specified location asynchronously
        /// </summary>
        /// <param name="location">The asset location to get assets for</param>
        /// <param name="user">The current user context</param>
        /// <returns>Assets for the specified location</returns>
        Task<IEnumerable<Asset>> GetAssetsAsync(AssetLocation location, ClaimsPrincipal? user = null);

        /// <summary>
        /// Gets all assets provided by this provider
        /// </summary>
        /// <returns>All assets from this provider</returns>
        IEnumerable<Asset> GetAllAssets();

        /// <summary>
        /// Gets all assets provided by this provider asynchronously
        /// </summary>
        /// <param name="user">The current user context</param>
        /// <returns>All assets from this provider</returns>
        Task<IEnumerable<Asset>> GetAllAssetsAsync(ClaimsPrincipal? user = null);

        /// <summary>
        /// Determines whether this provider supports the specified location
        /// </summary>
        /// <param name="location">The asset location to check</param>
        /// <returns>True if the location is supported, false otherwise</returns>
        bool SupportsLocation(AssetLocation location);

        /// <summary>
        /// Gets assets of a specific type for the specified location
        /// </summary>
        /// <param name="location">The asset location</param>
        /// <param name="assetType">The type of assets to retrieve</param>
        /// <returns>Assets of the specified type for the location</returns>
        IEnumerable<Asset> GetAssetsByType(AssetLocation location, AssetType assetType);

        /// <summary>
        /// Gets assets of a specific type for the specified location asynchronously
        /// </summary>
        /// <param name="location">The asset location</param>
        /// <param name="assetType">The type of assets to retrieve</param>
        /// <param name="user">The current user context</param>
        /// <returns>Assets of the specified type for the location</returns>
        Task<IEnumerable<Asset>> GetAssetsByTypeAsync(AssetLocation location, AssetType assetType, ClaimsPrincipal? user = null);
    }

    /// <summary>
    /// Base implementation of IAssetProvider that provides common functionality
    /// </summary>
    public abstract class AssetProviderBase : IAssetProvider
    {
        /// <summary>
        /// Gets the unique identifier for this asset provider
        /// </summary>
        public abstract string ProviderId { get; }

        /// <summary>
        /// Gets the priority of this asset provider (higher values = higher priority)
        /// </summary>
        public virtual int Priority => 0;

        /// <summary>
        /// Gets the asset locations that this provider supports
        /// </summary>
        public abstract IEnumerable<AssetLocation> SupportedLocations { get; }

        /// <summary>
        /// Gets assets for the specified location
        /// </summary>
        /// <param name="location">The asset location to get assets for</param>
        /// <returns>Assets for the specified location</returns>
        public virtual IEnumerable<Asset> GetAssets(AssetLocation location)
        {
            return GetAssetsAsync(location).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets assets for the specified location asynchronously
        /// </summary>
        /// <param name="location">The asset location to get assets for</param>
        /// <param name="user">The current user context</param>
        /// <returns>Assets for the specified location</returns>
        public abstract Task<IEnumerable<Asset>> GetAssetsAsync(AssetLocation location, ClaimsPrincipal? user = null);

        /// <summary>
        /// Gets all assets provided by this provider
        /// </summary>
        /// <returns>All assets from this provider</returns>
        public virtual IEnumerable<Asset> GetAllAssets()
        {
            return GetAllAssetsAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets all assets provided by this provider asynchronously
        /// </summary>
        /// <param name="user">The current user context</param>
        /// <returns>All assets from this provider</returns>
        public virtual async Task<IEnumerable<Asset>> GetAllAssetsAsync(ClaimsPrincipal? user = null)
        {
            var allAssets = new List<Asset>();

            foreach (var location in SupportedLocations)
            {
                var assets = await GetAssetsAsync(location, user);
                allAssets.AddRange(assets);
            }

            return allAssets.Distinct().ToList();
        }

        /// <summary>
        /// Determines whether this provider supports the specified location
        /// </summary>
        /// <param name="location">The asset location to check</param>
        /// <returns>True if the location is supported, false otherwise</returns>
        public virtual bool SupportsLocation(AssetLocation location)
        {
            return SupportedLocations.Contains(location);
        }

        /// <summary>
        /// Gets assets of a specific type for the specified location
        /// </summary>
        /// <param name="location">The asset location</param>
        /// <param name="assetType">The type of assets to retrieve</param>
        /// <returns>Assets of the specified type for the location</returns>
        public virtual IEnumerable<Asset> GetAssetsByType(AssetLocation location, AssetType assetType)
        {
            return GetAssetsByTypeAsync(location, assetType).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets assets of a specific type for the specified location asynchronously
        /// </summary>
        /// <param name="location">The asset location</param>
        /// <param name="assetType">The type of assets to retrieve</param>
        /// <param name="user">The current user context</param>
        /// <returns>Assets of the specified type for the location</returns>
        public virtual async Task<IEnumerable<Asset>> GetAssetsByTypeAsync(AssetLocation location, AssetType assetType, ClaimsPrincipal? user = null)
        {
            var assets = await GetAssetsAsync(location, user);
            return assets.Where(a => a.Type == assetType).ToList();
        }

        /// <summary>
        /// Sets common properties for assets created by this provider
        /// </summary>
        /// <param name="asset">The asset to configure</param>
        protected virtual void ConfigureAsset(Asset asset)
        {
            asset.ProviderId = ProviderId;
            
            if (string.IsNullOrEmpty(asset.Version))
            {
                asset.Version = GetDefaultVersion();
            }
        }

        /// <summary>
        /// Gets the default version for assets from this provider
        /// </summary>
        /// <returns>Default version string</returns>
        protected virtual string GetDefaultVersion()
        {
            return "1.0.0";
        }

        /// <summary>
        /// Creates a CSS asset with common configuration
        /// </summary>
        /// <param name="name">Asset name</param>
        /// <param name="href">CSS file path or URL</param>
        /// <param name="priority">Loading priority</param>
        /// <returns>Configured CSS asset</returns>
        protected CssAsset CreateCssAsset(string name, string href, int priority = 100)
        {
            var asset = new CssAsset(name, href)
            {
                Priority = priority
            };
            ConfigureAsset(asset);
            return asset;
        }

        /// <summary>
        /// Creates a JavaScript asset with common configuration
        /// </summary>
        /// <param name="name">Asset name</param>
        /// <param name="src">JavaScript file path or URL</param>
        /// <param name="priority">Loading priority</param>
        /// <param name="location">Where to render the script</param>
        /// <returns>Configured JavaScript asset</returns>
        protected JavaScriptAsset CreateJavaScriptAsset(string name, string src, int priority = 100, AssetLocation location = AssetLocation.Footer)
        {
            var asset = new JavaScriptAsset(name, src)
            {
                Priority = priority,
                Location = location
            };
            ConfigureAsset(asset);
            return asset;
        }

        /// <summary>
        /// Creates an inline CSS asset with common configuration
        /// </summary>
        /// <param name="name">Asset name</param>
        /// <param name="content">CSS content</param>
        /// <param name="priority">Loading priority</param>
        /// <returns>Configured inline CSS asset</returns>
        protected InlineCssAsset CreateInlineCssAsset(string name, string content, int priority = 100)
        {
            var asset = new InlineCssAsset(name, content)
            {
                Priority = priority
            };
            ConfigureAsset(asset);
            return asset;
        }

        /// <summary>
        /// Creates an inline JavaScript asset with common configuration
        /// </summary>
        /// <param name="name">Asset name</param>
        /// <param name="content">JavaScript content</param>
        /// <param name="priority">Loading priority</param>
        /// <param name="location">Where to render the script</param>
        /// <returns>Configured inline JavaScript asset</returns>
        protected InlineJavaScriptAsset CreateInlineJavaScriptAsset(string name, string content, int priority = 100, AssetLocation location = AssetLocation.InlineFooter)
        {
            var asset = new InlineJavaScriptAsset(name, content)
            {
                Priority = priority,
                Location = location
            };
            ConfigureAsset(asset);
            return asset;
        }
    }
} 