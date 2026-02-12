using System.Security.Claims;
using Bonyan.Ui.BonWeb.Mvc.Contracts;

namespace Bonyan.Ui.BonWeb.Mvc;

/// <summary>
/// Default implementation of BonWeb menu manager.
/// </summary>
public class BonWebMenuManager : IBonWebMenuManager
{
    private readonly List<IBonWebMenuProvider> _providers = new();

    public void RegisterProvider(IBonWebMenuProvider provider)
    {
        if (!_providers.Contains(provider))
            _providers.Add(provider);
    }

    public async Task<IEnumerable<BonWebMenuItem>> GetMenuItemsAsync(string location, ClaimsPrincipal? user = null)
    {
        var items = new List<BonWebMenuItem>();
        foreach (var provider in _providers.OrderByDescending(p => p.Priority))
        {
            if (!provider.SupportedLocations.Contains(location, StringComparer.OrdinalIgnoreCase))
                continue;
            var providerItems = await provider.GetMenuItemsAsync(location, user);
            items.AddRange(providerItems.Where(i => i.IsVisible(user)));
        }
        return items.OrderBy(i => i.Order).ToList();
    }
}
