using System.Security.Claims;
using Bonyan.Ui.BonWeb.Mvc.Contracts;
using Microsoft.Extensions.Options;

namespace Bonyan.Ui.BonWeb.Mvc;

/// <summary>
/// Default BonWeb asset provider: Bootstrap, jQuery, jQuery Validation, jQuery Unobtrusive, Bootstrap Icons.
/// Uses local files under <see cref="BonWebMvcOptions.LocalAssetBasePath"/> when <see cref="BonWebMvcOptions.UseLocalAssets"/> is true.
/// </summary>
public class BonWebDefaultAssetProvider : IBonWebAssetProvider
{
    private readonly BonWebMvcOptions _options;

    public BonWebDefaultAssetProvider(IOptions<BonWebMvcOptions> options)
    {
        _options = options?.Value ?? new BonWebMvcOptions();
    }

    public string ProviderId => "BonWeb.Default";
    public int Priority => 0;

    public IEnumerable<string> SupportedLocations => new[] { BonWebAssetLocations.Head, BonWebAssetLocations.Footer };

    public Task<IEnumerable<BonWebAssetDefinition>> GetAssetsAsync(string location, ClaimsPrincipal? user = null, BonWebAssetContext? context = null)
    {
        var assets = new List<BonWebAssetDefinition>();
        var basePath = _options.UseLocalAssets ? $"~/{_options.LocalAssetBasePath.TrimStart('/')}" : null;
        var isRtl = context?.IsRtl ?? false;

        if (location == BonWebAssetLocations.Head)
        {
            // Bootstrap and icons first (lower priority = loaded first)
            if (!string.IsNullOrEmpty(basePath))
            {
                assets.Add(new BonWebAssetDefinition("bonweb-bootstrap", BonWebAssetType.Css,
                    isRtl ? $"{basePath}/lib/bootstrap/css/bootstrap.rtl.min.css" : $"{basePath}/lib/bootstrap/css/bootstrap.min.css", 100));
                assets.Add(new BonWebAssetDefinition("bonweb-icons", BonWebAssetType.Css,
                    $"{basePath}/lib/bootstrap-icons/font/bootstrap-icons.min.css", 110));
                assets.Add(new BonWebAssetDefinition("bonweb-select2-css", BonWebAssetType.Css,
                    $"{basePath}/lib/select2/css/select2.min.css", 105));
            }
            else
            {
                assets.Add(new BonWebAssetDefinition("bonweb-bootstrap", BonWebAssetType.Css,
                    isRtl ? "https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.rtl.min.css" : "https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css", 100));
                assets.Add(new BonWebAssetDefinition("bonweb-icons", BonWebAssetType.Css,
                    "https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.1/font/bootstrap-icons.css", 110));
                assets.Add(new BonWebAssetDefinition("bonweb-select2-css", BonWebAssetType.Css,
                    "https://cdn.jsdelivr.net/npm/select2@4.0.13/dist/css/select2.min.css", 105));
            }

            // Fonts after Bootstrap (priority 115) so body font-family overrides Bootstrap's
            if (!string.IsNullOrEmpty(basePath))
            {
                var fontCss = isRtl
                    ? $"{basePath}/lib/fonts/rtl/fonts.css"
                    : $"{basePath}/lib/fonts/ltr/fonts.css";
                assets.Add(new BonWebAssetDefinition("bonweb-fonts", BonWebAssetType.Css, fontCss, 115));
            }
            else
            {
                var fontCssUrl = isRtl
                    ? "https://fonts.googleapis.com/css2?family=Vazirmatn:wght@400;500;600;700&display=swap"
                    : "https://fonts.googleapis.com/css2?family=Poppins:wght@400;500;600;700&display=swap";
                assets.Add(new BonWebAssetDefinition("bonweb-fonts", BonWebAssetType.Css, fontCssUrl, 115));
            }
        }
        else if (location == BonWebAssetLocations.Footer)
        {
            if (!string.IsNullOrEmpty(basePath))
            {
                assets.Add(new BonWebAssetDefinition("bonweb-jquery", BonWebAssetType.Script,
                    $"{basePath}/lib/jquery/jquery.min.js", 10));
                assets.Add(new BonWebAssetDefinition("bonweb-bootstrap-js", BonWebAssetType.Script,
                    $"{basePath}/lib/bootstrap/js/bootstrap.bundle.min.js", 20));
                assets.Add(new BonWebAssetDefinition("bonweb-jquery-validate", BonWebAssetType.Script,
                    $"{basePath}/lib/jquery-validate/jquery.validate.min.js", 30));
                assets.Add(new BonWebAssetDefinition("bonweb-jquery-unobtrusive", BonWebAssetType.Script,
                    $"{basePath}/lib/jquery-validation-unobtrusive/jquery.validate.unobtrusive.min.js", 40));
                assets.Add(new BonWebAssetDefinition("bonweb-select2", BonWebAssetType.Script,
                    $"{basePath}/lib/select2/js/select2.min.js", 45));
                assets.Add(new BonWebAssetDefinition("bonweb-sweetalert2", BonWebAssetType.Script,
                    $"{basePath}/lib/sweetalert2/sweetalert2.all.min.js", 50));
            }
            else
            {
                assets.Add(new BonWebAssetDefinition("bonweb-jquery", BonWebAssetType.Script,
                    "https://cdn.jsdelivr.net/npm/jquery@3.7.1/dist/jquery.min.js", 10));
                assets.Add(new BonWebAssetDefinition("bonweb-bootstrap-js", BonWebAssetType.Script,
                    "https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js", 20));
                assets.Add(new BonWebAssetDefinition("bonweb-jquery-validate", BonWebAssetType.Script,
                    "https://cdn.jsdelivr.net/npm/jquery-validation@1.19.5/dist/jquery.validate.min.js", 30));
                assets.Add(new BonWebAssetDefinition("bonweb-jquery-unobtrusive", BonWebAssetType.Script,
                    "https://cdn.jsdelivr.net/npm/jquery-validation-unobtrusive@4.0.0/dist/jquery.validate.unobtrusive.min.js", 40));
                assets.Add(new BonWebAssetDefinition("bonweb-select2", BonWebAssetType.Script,
                    "https://cdn.jsdelivr.net/npm/select2@4.0.13/dist/js/select2.min.js", 45));
                assets.Add(new BonWebAssetDefinition("bonweb-sweetalert2", BonWebAssetType.Script,
                    "https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.all.min.js", 50));
            }
        }

        return Task.FromResult<IEnumerable<BonWebAssetDefinition>>(assets);
    }
}
