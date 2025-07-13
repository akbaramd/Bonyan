using System.Security.Claims;
using Assets;

namespace Bonyan.Novino.Web.Assets
{
    public class DefaultAssetProvider : AssetProviderBase
    {
        public override string ProviderId => "Default";
        
        public override IEnumerable<AssetLocation> SupportedLocations => new[]
        {
            AssetLocation.Head,
            AssetLocation.Footer
        };
        
        public override Task<IEnumerable<Asset>> GetAssetsAsync(AssetLocation location, ClaimsPrincipal? user = null)
        {
            var assets = new List<Asset>();
            
            if (location == AssetLocation.Head)
            {
                // Bootstrap CSS (Head)
                assets.Add(CreateCssAsset("bootstrap-css", "/lib/bootstrap/dist/css/bootstrap.min.css", 100));

                // Font Awesome CSS (Head)
                assets.Add(CreateCssAsset("font-awesome-css", "https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css", 200));

                // Site CSS (Head)
                assets.Add(CreateCssAsset("site-css", "/css/site.css", 300));

                // Site Styles CSS (Head)
                var siteStylesAsset = CreateCssAsset("site-styles-css", "/Bonyan.Novino.Web.styles.css", 400);
                siteStylesAsset.Version = "1.0.0";
                assets.Add(siteStylesAsset);
            }
            else if (location == AssetLocation.Footer)
            {
                // jQuery JavaScript (Footer - before Bootstrap)
                assets.Add(CreateJavaScriptAsset("jquery-js", "/lib/jquery/dist/jquery.min.js", 100, AssetLocation.Footer));

                // Bootstrap JavaScript (Footer - after jQuery)
                var bootstrapAsset = CreateJavaScriptAsset("bootstrap-js", "/lib/bootstrap/dist/js/bootstrap.bundle.min.js", 200, AssetLocation.Footer);
                bootstrapAsset.Dependencies.Add("jquery-js");
                assets.Add(bootstrapAsset);

                // Site JavaScript (Footer - after Bootstrap)
                var siteAsset = CreateJavaScriptAsset("site-js", "/js/site.js", 300, AssetLocation.Footer);
                siteAsset.Dependencies.AddRange(new[] { "jquery-js", "bootstrap-js" });
                siteAsset.Version = "1.0.0";
                assets.Add(siteAsset);
            }
            
            return Task.FromResult<IEnumerable<Asset>>(assets);
        }
    }
} 