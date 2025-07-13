using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Assets
{
    /// <summary>
    /// Defines a contract for the asset manager that orchestrates asset providers
    /// </summary>
    public interface IAssetManager
    {
        /// <summary>
        /// Gets all registered asset providers
        /// </summary>
        IEnumerable<IAssetProvider> AssetProviders { get; }

        /// <summary>
        /// Gets assets for a specific location
        /// </summary>
        /// <param name="location">The asset location</param>
        /// <param name="user">The current user context</param>
        /// <returns>Assets for the specified location, sorted by priority and dependencies</returns>
        Task<IEnumerable<Asset>> GetAssetsAsync(AssetLocation location, ClaimsPrincipal? user = null);

        /// <summary>
        /// Gets assets for a specific location synchronously
        /// </summary>
        /// <param name="location">The asset location</param>
        /// <param name="user">The current user context</param>
        /// <returns>Assets for the specified location, sorted by priority and dependencies</returns>
        IEnumerable<Asset> GetAssets(AssetLocation location, ClaimsPrincipal? user = null);

        /// <summary>
        /// Gets assets of a specific type for a location
        /// </summary>
        /// <param name="location">The asset location</param>
        /// <param name="assetType">The type of assets to retrieve</param>
        /// <param name="user">The current user context</param>
        /// <returns>Assets of the specified type for the location</returns>
        Task<IEnumerable<Asset>> GetAssetsByTypeAsync(AssetLocation location, AssetType assetType, ClaimsPrincipal? user = null);

        /// <summary>
        /// Gets assets of a specific type for a location synchronously
        /// </summary>
        /// <param name="location">The asset location</param>
        /// <param name="assetType">The type of assets to retrieve</param>
        /// <param name="user">The current user context</param>
        /// <returns>Assets of the specified type for the location</returns>
        IEnumerable<Asset> GetAssetsByType(AssetLocation location, AssetType assetType, ClaimsPrincipal? user = null);

        /// <summary>
        /// Gets all assets from all providers
        /// </summary>
        /// <param name="user">The current user context</param>
        /// <returns>All assets from all providers</returns>
        Task<IEnumerable<Asset>> GetAllAssetsAsync(ClaimsPrincipal? user = null);

        /// <summary>
        /// Gets all assets from all providers synchronously
        /// </summary>
        /// <param name="user">The current user context</param>
        /// <returns>All assets from all providers</returns>
        IEnumerable<Asset> GetAllAssets(ClaimsPrincipal? user = null);

        /// <summary>
        /// Renders assets for a specific location as HTML
        /// </summary>
        /// <param name="location">The asset location</param>
        /// <param name="user">The current user context</param>
        /// <returns>HTML string containing all assets for the location</returns>
        Task<string> RenderAssetsAsync(AssetLocation location, ClaimsPrincipal? user = null);

        /// <summary>
        /// Renders assets for a specific location as HTML synchronously
        /// </summary>
        /// <param name="location">The asset location</param>
        /// <param name="user">The current user context</param>
        /// <returns>HTML string containing all assets for the location</returns>
        string RenderAssets(AssetLocation location, ClaimsPrincipal? user = null);

        /// <summary>
        /// Renders assets of a specific type for a location as HTML
        /// </summary>
        /// <param name="location">The asset location</param>
        /// <param name="assetType">The type of assets to render</param>
        /// <param name="user">The current user context</param>
        /// <returns>HTML string containing assets of the specified type</returns>
        Task<string> RenderAssetsByTypeAsync(AssetLocation location, AssetType assetType, ClaimsPrincipal? user = null);

        /// <summary>
        /// Renders assets of a specific type for a location as HTML synchronously
        /// </summary>
        /// <param name="location">The asset location</param>
        /// <param name="assetType">The type of assets to render</param>
        /// <param name="user">The current user context</param>
        /// <returns>HTML string containing assets of the specified type</returns>
        string RenderAssetsByType(AssetLocation location, AssetType assetType, ClaimsPrincipal? user = null);

        /// <summary>
        /// Registers an asset provider
        /// </summary>
        /// <param name="provider">The asset provider to register</param>
        void RegisterProvider(IAssetProvider provider);

        /// <summary>
        /// Unregisters an asset provider
        /// </summary>
        /// <param name="provider">The asset provider to unregister</param>
        void UnregisterProvider(IAssetProvider provider);

        /// <summary>
        /// Gets providers that support a specific location
        /// </summary>
        /// <param name="location">The asset location</param>
        /// <returns>Providers that support the specified location</returns>
        IEnumerable<IAssetProvider> GetProvidersForLocation(AssetLocation location);

        /// <summary>
        /// Adds a single asset to a specific location
        /// </summary>
        /// <param name="location">The asset location</param>
        /// <param name="asset">The asset to add</param>
        void AddAsset(AssetLocation location, Asset asset);

        /// <summary>
        /// Removes an asset by ID
        /// </summary>
        /// <param name="assetId">The ID of the asset to remove</param>
        /// <returns>True if the asset was removed, false if not found</returns>
        bool RemoveAsset(string assetId);

        /// <summary>
        /// Gets an asset by ID
        /// </summary>
        /// <param name="assetId">The asset ID</param>
        /// <returns>The asset if found, null otherwise</returns>
        Asset? GetAssetById(string assetId);
    }
} 