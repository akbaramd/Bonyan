using System.Security.Claims;

namespace Bonyan.Ui.BonWeb.Mvc.Contracts;

/// <summary>
/// Contract for providers that contribute menu items to the BonWeb dashboard.
/// </summary>
public interface IBonWebMenuProvider
{
    /// <summary>
    /// Unique identifier for this provider.
    /// </summary>
    string ProviderId { get; }

    /// <summary>
    /// Priority (higher = rendered first).
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// Menu locations this provider supports (e.g. BonWebMenuLocations.Sidebar).
    /// </summary>
    IEnumerable<string> SupportedLocations { get; }

    /// <summary>
    /// Gets menu items for a location.
    /// </summary>
    Task<IEnumerable<BonWebMenuItem>> GetMenuItemsAsync(string location, ClaimsPrincipal? user = null);
}
