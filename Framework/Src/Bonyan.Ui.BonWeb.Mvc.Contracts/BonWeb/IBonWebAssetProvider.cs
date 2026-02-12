using System.Security.Claims;

namespace Bonyan.Ui.BonWeb.Mvc.Contracts;

/// <summary>
/// Contract for providers that contribute assets (CSS/JS) to the BonWeb dashboard.
/// When <paramref name="context"/> is provided, check <see cref="BonWebAssetContext.IsRtl"/> to add RTL versions.
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
    /// Gets assets for a location. When <paramref name="context"/> is set, use <see cref="BonWebAssetContext.IsRtl"/> to add RTL assets (e.g. bootstrap.rtl.min.css).
    /// </summary>
    Task<IEnumerable<BonWebAssetDefinition>> GetAssetsAsync(string location, ClaimsPrincipal? user = null, BonWebAssetContext? context = null);
}
