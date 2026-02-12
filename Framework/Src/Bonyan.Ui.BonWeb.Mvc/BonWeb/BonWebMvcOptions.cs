namespace Bonyan.Ui.BonWeb.Mvc;

/// <summary>
/// Options for BonWeb MVC UI: local vs CDN assets and asset base path.
/// Projects that start with this framework use these for Bootstrap, jQuery, and validation scripts.
/// </summary>
public class BonWebMvcOptions
{
    /// <summary>
    /// When true, Bootstrap/jQuery/jQuery.Validation/jQuery.Unobtrusive are loaded from local files
    /// under <see cref="LocalAssetBasePath"/>. When false, CDN URLs are used.
    /// </summary>
    public bool UseLocalAssets { get; set; } = true;

    /// <summary>
    /// Base path for local assets (e.g. "_content/Bonyan.Ui.BonWeb.Mvc" for RCL static files).
    /// Scripts and styles are under this path, e.g. lib/bootstrap/, lib/jquery/.
    /// </summary>
    public string LocalAssetBasePath { get; set; } = "_content/Bonyan.Ui.BonWeb.Mvc";
}
