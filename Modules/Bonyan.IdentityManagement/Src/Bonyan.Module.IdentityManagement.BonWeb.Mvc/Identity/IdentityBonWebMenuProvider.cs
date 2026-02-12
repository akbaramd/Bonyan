using System.Security.Claims;
using Bonyan.Ui.BonWeb.Mvc.Contracts;

namespace Bonyan.IdentityManagement.BonWeb.Mvc;

/// <summary>
/// Menu provider that adds Identity Management items (Users, Roles) to the BonWeb sidebar.
/// </summary>
public class IdentityBonWebMenuProvider : IBonWebMenuProvider
{
    public string ProviderId => "IdentityManagement.BonWeb";
    public int Priority => 10;

    public IEnumerable<string> SupportedLocations => new[] { BonWebMenuLocations.Sidebar };

    public Task<IEnumerable<BonWebMenuItem>> GetMenuItemsAsync(string location, ClaimsPrincipal? user = null)
    {
        var identityGroup = new BonWebMenuItem("Identity", "#", "bi-shield-lock", 100)
        {
            Children = new List<BonWebMenuItem>
            {
                new BonWebMenuItem("Users", "/IdentityManagement/Users", "bi-people", 1)
                {
                    RequiresAuthentication = true
                },
                new BonWebMenuItem("Roles", "/IdentityManagement/Roles", "bi-person-badge", 2)
                {
                    RequiresAuthentication = true
                }
            }
        };
        return Task.FromResult<IEnumerable<BonWebMenuItem>>(new[] { identityGroup });
    }
}
