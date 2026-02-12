using Microsoft.AspNetCore.Mvc;
using Bonyan.Ui.BonWeb.Mvc.Contracts;

namespace Bonyan.Ui.BonWeb.Mvc.ViewComponents;

/// <summary>
/// Renders the BonWeb sidebar menu.
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
        var items = await _menuManager.GetMenuItemsAsync(BonWebMenuLocations.Sidebar, user);
        return View(items);
    }
}
