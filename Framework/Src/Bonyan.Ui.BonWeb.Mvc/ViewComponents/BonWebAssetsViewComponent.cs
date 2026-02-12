using Microsoft.AspNetCore.Mvc;
using Bonyan.Ui.BonWeb.Mvc.Contracts;

namespace Bonyan.Ui.BonWeb.Mvc.ViewComponents;

/// <summary>
/// Renders BonWeb assets (CSS/JS) for a given location.
/// </summary>
public class BonWebAssetsViewComponent : ViewComponent
{
    private readonly IBonWebAssetManager _assetManager;

    public BonWebAssetsViewComponent(IBonWebAssetManager assetManager)
    {
        _assetManager = assetManager;
    }

    public async Task<IViewComponentResult> InvokeAsync(string location)
    {
        var user = HttpContext.User;
        var assets = await _assetManager.GetAssetsAsync(location, user);
        return View(assets);
    }
}
