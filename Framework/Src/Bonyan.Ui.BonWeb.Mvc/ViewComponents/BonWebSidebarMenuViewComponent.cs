using Microsoft.AspNetCore.Mvc;
using Bonyan.Ui.BonWeb.Mvc.Contracts;

namespace Bonyan.Ui.BonWeb.Mvc.ViewComponents;

/// <summary>
/// Renders the BonWeb sidebar menu with optional groups and nested items.
/// </summary>
public class BonWebSidebarMenuViewComponent : ViewComponent
{
    private readonly IBonWebMenuManager _menuManager;

    public BonWebSidebarMenuViewComponent(IBonWebMenuManager menuManager)
    {
        _menuManager = menuManager;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var user = HttpContext.User;
        var items = (await _menuManager.GetMenuItemsAsync(BonWebMenuLocations.Sidebar, user)).ToList();
        var groups = BuildGroups(items);
        return View(groups);
    }

    private static List<BonWebMenuGroup> BuildGroups(List<BonWebMenuItem> items)
    {
        var grouped = items
            .GroupBy(i => string.IsNullOrEmpty(i.GroupName) ? null : i.GroupName)
            .OrderBy(g => g.Key == null ? "" : g.Key)
            .Select(g => new BonWebMenuGroup
            {
                GroupName = g.Key,
                Items = g.OrderBy(x => x.Order).ToList()
            })
            .ToList();
        return grouped;
    }
}
