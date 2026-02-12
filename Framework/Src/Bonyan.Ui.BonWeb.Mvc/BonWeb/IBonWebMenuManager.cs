using System.Security.Claims;
using Bonyan.Ui.BonWeb.Mvc.Contracts;

namespace Bonyan.Ui.BonWeb.Mvc;

/// <summary>
/// Manages menu providers and resolves menu items for the dashboard.
/// </summary>
public interface IBonWebMenuManager
{
    void RegisterProvider(IBonWebMenuProvider provider);
    Task<IEnumerable<BonWebMenuItem>> GetMenuItemsAsync(string location, ClaimsPrincipal? user = null);
}
