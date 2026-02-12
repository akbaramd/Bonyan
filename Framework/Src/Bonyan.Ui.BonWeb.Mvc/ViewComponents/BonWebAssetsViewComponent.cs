using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Bonyan.Ui.BonWeb.Mvc.Contracts;

namespace Bonyan.Ui.BonWeb.Mvc.ViewComponents;

/// <summary>
/// Renders BonWeb assets (CSS/JS) for a given location. Passes RTL context so providers can add RTL versions.
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
        var culture = CultureInfo.CurrentUICulture;
        var context = new BonWebAssetContext
        {
            IsRtl = culture.TextInfo.IsRightToLeft,
            CultureName = culture.Name,
            LanguageCode = culture.TwoLetterISOLanguageName
        };
        var assets = await _assetManager.GetAssetsAsync(location, user, context);
        return View(assets);
    }
}
