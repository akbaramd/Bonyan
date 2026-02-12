using System.Security.Claims;
using Bonyan.Ui.BonWeb.Mvc.Contracts;
using Microsoft.Extensions.Localization;

namespace Bonyan.Ui.BonWeb.Mvc;

/// <summary>
/// Default BonWeb menu provider - adds Dashboard home link.
/// </summary>
public class BonWebDefaultMenuProvider : IBonWebMenuProvider
{
    private readonly IStringLocalizer<BonWebMenuResource> _localizer;

    public BonWebDefaultMenuProvider(IStringLocalizer<BonWebMenuResource> localizer)
    {
        _localizer = localizer;
    }

    public string ProviderId => "BonWeb.Default";
    public int Priority => 0;

    public IEnumerable<string> SupportedLocations => new[] { BonWebMenuLocations.Sidebar };

    public Task<IEnumerable<BonWebMenuItem>> GetMenuItemsAsync(string location, ClaimsPrincipal? user = null)
    {
        var items = new List<BonWebMenuItem>
        {
            new BonWebMenuItem(_localizer["Menu:Dashboard"].Value, "/", "bi-house", 0) { RequiresAuthentication = true }
        };
        return Task.FromResult<IEnumerable<BonWebMenuItem>>(items);
    }
}
