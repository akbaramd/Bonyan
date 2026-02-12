using System.Security.Claims;
using Bonyan.Ui.BonWeb.Mvc.Contracts;

namespace Bonyan.Ui.BonWeb.Mvc;

/// <summary>
/// Default BonWeb asset provider - base dashboard CSS and optional Bootstrap.
/// </summary>
public class BonWebDefaultAssetProvider : IBonWebAssetProvider
{
    public string ProviderId => "BonWeb.Default";
    public int Priority => 0;

    public IEnumerable<string> SupportedLocations => new[] { BonWebAssetLocations.Head, BonWebAssetLocations.Footer };

    public Task<IEnumerable<BonWebAssetDefinition>> GetAssetsAsync(string location, ClaimsPrincipal? user = null)
    {
        var assets = new List<BonWebAssetDefinition>();
        if (location == BonWebAssetLocations.Head)
        {
            assets.Add(new BonWebAssetDefinition("bonweb-bootstrap", BonWebAssetType.Css,
                "https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css", 100));
            assets.Add(new BonWebAssetDefinition("bonweb-icons", BonWebAssetType.Css,
                "https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.1/font/bootstrap-icons.css", 110));
        }
        else if (location == BonWebAssetLocations.Footer)
        {
            assets.Add(new BonWebAssetDefinition("bonweb-bootstrap-js", BonWebAssetType.Script,
                "https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js", 100));
        }
        return Task.FromResult<IEnumerable<BonWebAssetDefinition>>(assets);
    }
}
