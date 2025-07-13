using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Assets
{
    /// <summary>
    /// Default implementation of IAssetManager that orchestrates asset providers
    /// </summary>
    public class AssetManager : IAssetManager
    {
        private readonly ILogger<AssetManager> _logger;
        private readonly ConcurrentDictionary<string, IAssetProvider> _providers = new();
        private readonly ConcurrentDictionary<string, Asset> _manualAssets = new();

        /// <summary>
        /// Gets all registered asset providers
        /// </summary>
        public IEnumerable<IAssetProvider> AssetProviders => _providers.Values;

        public AssetManager(ILogger<AssetManager> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Gets assets for a specific location
        /// </summary>
        /// <param name="location">The asset location</param>
        /// <param name="user">The current user context</param>
        /// <returns>Assets for the specified location, sorted by priority and dependencies</returns>
        public async Task<IEnumerable<Asset>> GetAssetsAsync(AssetLocation location, ClaimsPrincipal? user = null)
        {
            try
            {
                var allAssets = new List<Asset>();

                // Get assets from all providers that support this location
                var providers = GetProvidersForLocation(location);
                foreach (var provider in providers.OrderByDescending(p => p.Priority))
                {
                    try
                    {
                        var assets = await provider.GetAssetsAsync(location, user);
                        allAssets.AddRange(assets.Where(a => a.IsEnabled));
                        
                        _logger.LogDebug("Provider '{ProviderId}' contributed {Count} assets for location '{Location}'", 
                            provider.ProviderId, assets.Count(), location);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error getting assets from provider '{ProviderId}' for location '{Location}'", 
                            provider.ProviderId, location);
                    }
                }

                // Add manually added assets for this location
                var manualAssets = _manualAssets.Values.Where(a => a.Location == location && a.IsEnabled);
                allAssets.AddRange(manualAssets);

                // Remove duplicates (by ID)
                var uniqueAssets = allAssets
                    .GroupBy(a => a.Id)
                    .Select(g => g.OrderByDescending(a => a.Priority).First())
                    .ToList();

                // Sort by dependencies and priority
                var sortedAssets = SortAssetsByDependencies(uniqueAssets);

                return sortedAssets;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting assets for location '{Location}'", location);
                return Enumerable.Empty<Asset>();
            }
        }

        /// <summary>
        /// Gets assets for a specific location synchronously
        /// </summary>
        /// <param name="location">The asset location</param>
        /// <param name="user">The current user context</param>
        /// <returns>Assets for the specified location, sorted by priority and dependencies</returns>
        public IEnumerable<Asset> GetAssets(AssetLocation location, ClaimsPrincipal? user = null)
        {
            return GetAssetsAsync(location, user).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets assets of a specific type for a location
        /// </summary>
        /// <param name="location">The asset location</param>
        /// <param name="assetType">The type of assets to retrieve</param>
        /// <param name="user">The current user context</param>
        /// <returns>Assets of the specified type for the location</returns>
        public async Task<IEnumerable<Asset>> GetAssetsByTypeAsync(AssetLocation location, AssetType assetType, ClaimsPrincipal? user = null)
        {
            var allAssets = await GetAssetsAsync(location, user);
            return allAssets.Where(a => a.Type == assetType).ToList();
        }

        /// <summary>
        /// Gets assets of a specific type for a location synchronously
        /// </summary>
        /// <param name="location">The asset location</param>
        /// <param name="assetType">The type of assets to retrieve</param>
        /// <param name="user">The current user context</param>
        /// <returns>Assets of the specified type for the location</returns>
        public IEnumerable<Asset> GetAssetsByType(AssetLocation location, AssetType assetType, ClaimsPrincipal? user = null)
        {
            return GetAssetsByTypeAsync(location, assetType, user).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets all assets from all providers
        /// </summary>
        /// <param name="user">The current user context</param>
        /// <returns>All assets from all providers</returns>
        public async Task<IEnumerable<Asset>> GetAllAssetsAsync(ClaimsPrincipal? user = null)
        {
            var allAssets = new List<Asset>();

            try
            {
                // Get assets from all providers
                foreach (var provider in AssetProviders)
                {
                    try
                    {
                        var assets = await provider.GetAllAssetsAsync(user);
                        allAssets.AddRange(assets.Where(a => a.IsEnabled));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error getting all assets from provider '{ProviderId}'", provider.ProviderId);
                    }
                }

                // Add manually added assets
                allAssets.AddRange(_manualAssets.Values.Where(a => a.IsEnabled));

                // Remove duplicates
                var uniqueAssets = allAssets
                    .GroupBy(a => a.Id)
                    .Select(g => g.OrderByDescending(a => a.Priority).First())
                    .ToList();

                return uniqueAssets;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all assets");
                return Enumerable.Empty<Asset>();
            }
        }

        /// <summary>
        /// Gets all assets from all providers synchronously
        /// </summary>
        /// <param name="user">The current user context</param>
        /// <returns>All assets from all providers</returns>
        public IEnumerable<Asset> GetAllAssets(ClaimsPrincipal? user = null)
        {
            return GetAllAssetsAsync(user).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Renders assets for a specific location as HTML
        /// </summary>
        /// <param name="location">The asset location</param>
        /// <param name="user">The current user context</param>
        /// <returns>HTML string containing all assets for the location</returns>
        public async Task<string> RenderAssetsAsync(AssetLocation location, ClaimsPrincipal? user = null)
        {
            var assets = await GetAssetsAsync(location, user);
            return RenderAssetsToHtml(assets);
        }

        /// <summary>
        /// Renders assets for a specific location as HTML synchronously
        /// </summary>
        /// <param name="location">The asset location</param>
        /// <param name="user">The current user context</param>
        /// <returns>HTML string containing all assets for the location</returns>
        public string RenderAssets(AssetLocation location, ClaimsPrincipal? user = null)
        {
            return RenderAssetsAsync(location, user).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Renders assets of a specific type for a location as HTML
        /// </summary>
        /// <param name="location">The asset location</param>
        /// <param name="assetType">The type of assets to render</param>
        /// <param name="user">The current user context</param>
        /// <returns>HTML string containing assets of the specified type</returns>
        public async Task<string> RenderAssetsByTypeAsync(AssetLocation location, AssetType assetType, ClaimsPrincipal? user = null)
        {
            var assets = await GetAssetsByTypeAsync(location, assetType, user);
            return RenderAssetsToHtml(assets);
        }

        /// <summary>
        /// Renders assets of a specific type for a location as HTML synchronously
        /// </summary>
        /// <param name="location">The asset location</param>
        /// <param name="assetType">The type of assets to render</param>
        /// <param name="user">The current user context</param>
        /// <returns>HTML string containing assets of the specified type</returns>
        public string RenderAssetsByType(AssetLocation location, AssetType assetType, ClaimsPrincipal? user = null)
        {
            return RenderAssetsByTypeAsync(location, assetType, user).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Registers an asset provider
        /// </summary>
        /// <param name="provider">The asset provider to register</param>
        public void RegisterProvider(IAssetProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            _providers.TryAdd(provider.ProviderId, provider);
            _logger.LogInformation("Registered asset provider '{ProviderId}' supporting locations: {Locations}", 
                provider.ProviderId, string.Join(", ", provider.SupportedLocations));
        }

        /// <summary>
        /// Unregisters an asset provider
        /// </summary>
        /// <param name="provider">The asset provider to unregister</param>
        public void UnregisterProvider(IAssetProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            if (_providers.TryRemove(provider.ProviderId, out _))
            {
                _logger.LogInformation("Unregistered asset provider '{ProviderId}'", provider.ProviderId);
            }
        }

        /// <summary>
        /// Gets providers that support a specific location
        /// </summary>
        /// <param name="location">The asset location</param>
        /// <returns>Providers that support the specified location</returns>
        public IEnumerable<IAssetProvider> GetProvidersForLocation(AssetLocation location)
        {
            return AssetProviders.Where(p => p.SupportsLocation(location));
        }

        /// <summary>
        /// Adds a single asset to a specific location
        /// </summary>
        /// <param name="location">The asset location</param>
        /// <param name="asset">The asset to add</param>
        public void AddAsset(AssetLocation location, Asset asset)
        {
            if (asset == null)
                throw new ArgumentNullException(nameof(asset));

            asset.Location = location;
            _manualAssets.TryAdd(asset.Id, asset);
            
            _logger.LogDebug("Added manual asset '{AssetName}' to location '{Location}'", asset.Name, location);
        }

        /// <summary>
        /// Removes an asset by ID
        /// </summary>
        /// <param name="assetId">The ID of the asset to remove</param>
        /// <returns>True if the asset was removed, false if not found</returns>
        public bool RemoveAsset(string assetId)
        {
            if (string.IsNullOrEmpty(assetId))
                return false;

            var removed = _manualAssets.TryRemove(assetId, out var asset);
            if (removed && asset != null)
            {
                _logger.LogDebug("Removed manual asset '{AssetName}' ({AssetId})", asset.Name, assetId);
            }
            
            return removed;
        }

        /// <summary>
        /// Gets an asset by ID
        /// </summary>
        /// <param name="assetId">The asset ID</param>
        /// <returns>The asset if found, null otherwise</returns>
        public Asset? GetAssetById(string assetId)
        {
            if (string.IsNullOrEmpty(assetId))
                return null;

            return _manualAssets.TryGetValue(assetId, out var asset) ? asset : null;
        }

        /// <summary>
        /// Sorts assets by their dependencies and priority
        /// </summary>
        /// <param name="assets">The assets to sort</param>
        /// <returns>Sorted assets</returns>
        private List<Asset> SortAssetsByDependencies(List<Asset> assets)
        {
            var sorted = new List<Asset>();
            var remaining = new List<Asset>(assets);
            var processed = new HashSet<string>();

            // Create a lookup for faster dependency resolution
            var assetLookup = assets.ToDictionary(a => a.Id, a => a);

            while (remaining.Any())
            {
                var beforeCount = remaining.Count;

                // Find assets with no unresolved dependencies
                var readyAssets = remaining
                    .Where(asset => asset.Dependencies.All(depId => 
                        processed.Contains(depId) || !assetLookup.ContainsKey(depId)))
                    .OrderBy(asset => asset.Priority)
                    .ThenBy(asset => asset.Name)
                    .ToList();

                if (!readyAssets.Any())
                {
                    // Circular dependency or missing dependency - log warning and add remaining assets
                    _logger.LogWarning("Possible circular dependencies or missing dependencies detected in assets. Adding remaining {Count} assets without dependency ordering.", remaining.Count);
                    readyAssets = remaining.OrderBy(a => a.Priority).ThenBy(a => a.Name).ToList();
                }

                foreach (var asset in readyAssets)
                {
                    sorted.Add(asset);
                    processed.Add(asset.Id);
                    remaining.Remove(asset);
                }

                // Prevent infinite loop
                if (remaining.Count == beforeCount && readyAssets.Any())
                {
                    break;
                }
            }

            return sorted;
        }

        /// <summary>
        /// Renders a collection of assets to HTML
        /// </summary>
        /// <param name="assets">The assets to render</param>
        /// <returns>HTML string</returns>
        private string RenderAssetsToHtml(IEnumerable<Asset> assets)
        {
            var html = new StringBuilder();

            foreach (var asset in assets)
            {
                try
                {
                    var renderedAsset = asset.Render();
                    if (!string.IsNullOrEmpty(renderedAsset))
                    {
                        html.AppendLine(renderedAsset);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error rendering asset '{AssetName}' ({AssetId})", asset.Name, asset.Id);
                }
            }

            return html.ToString();
        }
    }
} 