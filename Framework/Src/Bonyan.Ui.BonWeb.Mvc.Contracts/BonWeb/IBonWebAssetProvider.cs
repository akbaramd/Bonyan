using System.Security.Claims;

namespace Bonyan.Ui.BonWeb.Mvc.Contracts;

/// <summary>
/// Contract for providers that contribute assets (CSS/JS) to the BonWeb dashboard.
/// </summary>
public interface IBonWebAssetProvider
{
    /// <summary>
    /// Unique identifier for this provider.
    /// </summary>
    string ProviderId { get; }

    /// <summary>
    /// Priority (higher = loaded first).
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// Asset locations this provider supports (e.g. BonWebAssetLocations.Head).
    /// </summary>
    IEnumerable<string> SupportedLocations { get; }

    /// <summary>
    /// Gets assets for a location.
    /// </summary>
    Task<IEnumerable<BonWebAssetDefinition>> GetAssetsAsync(string location, ClaimsPrincipal? user = null);
}
