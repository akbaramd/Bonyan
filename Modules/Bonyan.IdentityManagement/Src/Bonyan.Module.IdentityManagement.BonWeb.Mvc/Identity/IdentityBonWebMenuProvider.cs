using System.Security.Claims;
using Bonyan.Ui.BonWeb.Mvc.Contracts;
using Bonyan.Ui.BonWeb.Mvc;
using Microsoft.Extensions.Localization;

namespace Bonyan.IdentityManagement.BonWeb.Mvc;

/// <summary>
/// Menu provider that adds Identity Management items (Users, Roles) to the BonWeb sidebar.
/// </summary>
public class IdentityBonWebMenuProvider : IBonWebMenuProvider
{
    private readonly IStringLocalizer<BonWebMenuResource> _localizer;

    public IdentityBonWebMenuProvider(IStringLocalizer<BonWebMenuResource> localizer)
    {
        _localizer = localizer;
    }

    public string ProviderId => "IdentityManagement.BonWeb";
    public int Priority => 10;

    public IEnumerable<string> SupportedLocations => new[] { BonWebMenuLocations.Sidebar };

    public Task<IEnumerable<BonWebMenuItem>> GetMenuItemsAsync(string location, ClaimsPrincipal? user = null)
    {
        var groupName = _localizer["Menu:System"].Value;
        var items = new List<BonWebMenuItem>
        {
            new BonWebMenuItem(_localizer["Menu:Users"].Value, "/IdentityManagement/Users", "bi-people", 1)
            {
                GroupName = groupName,
                RequiresAuthentication = true
            },
            new BonWebMenuItem(_localizer["Menu:Roles"].Value, "/IdentityManagement/Roles", "bi-person-badge", 2)
            {
                GroupName = groupName,
                RequiresAuthentication = true
            }
        };
        return Task.FromResult<IEnumerable<BonWebMenuItem>>(items);
    }
}
