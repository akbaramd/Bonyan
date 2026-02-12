using System.Security.Claims;
using Bonyan.Ui.BonWeb.Mvc.Contracts;

namespace Bonyan.Ui.BonWeb.Mvc;

/// <summary>
/// Manages asset providers and resolves assets for the dashboard.
/// </summary>
public interface IBonWebAssetManager
{
    void RegisterProvider(IBonWebAssetProvider provider);
    Task<IEnumerable<BonWebAssetDefinition>> GetAssetsAsync(string location, ClaimsPrincipal? user = null);
}
