using System.Security.Claims;
using Bonyan.Ui.BonWeb.Mvc.Contracts;

namespace Bonyan.Ui.BonWeb.Mvc;

/// <summary>
/// Default implementation of BonWeb asset manager.
/// </summary>
public class BonWebAssetManager : IBonWebAssetManager
{
    private readonly List<IBonWebAssetProvider> _providers = new();

    public void RegisterProvider(IBonWebAssetProvider provider)
    {
        if (!_providers.Contains(provider))
            _providers.Add(provider);
    }

    public async Task<IEnumerable<BonWebAssetDefinition>> GetAssetsAsync(string location, ClaimsPrincipal? user = null)
    {
        var assets = new List<BonWebAssetDefinition>();
        foreach (var provider in _providers.OrderByDescending(p => p.Priority))
        {
            if (!provider.SupportedLocations.Contains(location, StringComparer.OrdinalIgnoreCase))
                continue;
            var providerAssets = await provider.GetAssetsAsync(location, user);
            assets.AddRange(providerAssets);
        }
        return assets.OrderBy(a => a.Priority).ToList();
    }
}
