using System.Security.Claims;
using Bonyan.Novino.Core.Assets;

namespace Bonyan.Novino.Web.Assets
{
    public class DefaultAssetProvider : AssetProviderBase
    {
        public override string ProviderId => "Default";
        
        public override IEnumerable<AssetLocation> SupportedLocations => new[]
        {
            AssetLocation.Head,
            AssetLocation.Footer,
            AssetLocation.InlineHead
        };
        
        public override Task<IEnumerable<Asset>> GetAssetsAsync(AssetLocation location, ClaimsPrincipal? user = null)
        {
            var assets = new List<Asset>();
            
            if (location == AssetLocation.Head)
            {
                // Layout config JS (Head - highest priority)
                assets.Add(CreateJavaScriptAsset("layout-js", "/assets/js/layout.js", 10, AssetLocation.Head));

                // Vazirmatn Font CSS (Head - early loading)
                assets.Add(CreateCssAsset("vazirmatn-font", "/assets/fonts/vazirmatn/Vazirmatn-font-face.css", 20));

                // Bootstrap RTL CSS (Head - core framework)
                assets.Add(CreateCssAsset("bootstrap-rtl-css", "/assets/css/bootstrap-rtl.min.css", 30));

                // Font Awesome CSS (Head - after Bootstrap)
                assets.Add(CreateCssAsset("font-awesome-css", "https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css", 35));

                // GridJS Theme CSS (Head - after Bootstrap)
                assets.Add(CreateCssAsset("gridjs-theme-css", "/assets/libs/gridjs/theme/mermaid.min.css", 40));

                // Toastify CSS (Head - after Bootstrap)
                assets.Add(CreateCssAsset("toastify-css", "/assets/libs/toastify-js/src/toastify.css", 50));

                // Icons CSS (Head - after Bootstrap)
                assets.Add(CreateCssAsset("icons-css", "/assets/css/icons.min.css", 60));

                // App RTL CSS (Head - main application styles)
                assets.Add(CreateCssAsset("app-rtl-css", "/assets/css/app-rtl.min.css", 70));

                // Custom CSS (Head - last to override others)
                assets.Add(CreateCssAsset("custom-css", "/assets/css/custom.css", 80));
            }
            else if (location == AssetLocation.InlineHead)
            {
                // Inline CSS for toast styling
                var toastStyles = @"
                    .toast {
                        background-color: #fff !important;
                        border: none !important;
                        box-shadow: 0 2px 5px rgba(0,0,0,0.2) !important;
                        border-radius: 4px !important;
                        padding: 15px !important;
                        min-width: 300px !important;
                        max-width: 400px !important;
                    }
                    .toast-success {
                        border-right: 4px solid #28a745 !important;
                    }
                    .toast-error {
                        border-right: 4px solid #dc3545 !important;
                    }
                    .toast-warning {
                        border-right: 4px solid #ffc107 !important;
                    }
                    .toast-info {
                        border-right: 4px solid #17a2b8 !important;
                    }
                    .toast-message {
                        color: #333 !important;
                        font-size: 14px !important;
                        font-weight: 500 !important;
                        margin-right: 10px !important;
                    }
                    .toast-close-button {
                        color: #666 !important;
                        font-size: 18px !important;
                        font-weight: bold !important;
                        opacity: 0.5 !important;
                    }
                    .toast-close-button:hover {
                        opacity: 1 !important;
                    }
                    .toast-progress {
                        background-color: rgba(0,0,0,0.1) !important;
                    }
                    .toast-bottom-left {
                        bottom: 12px !important;
                        left: 12px !important;
                    }";

                assets.Add(CreateInlineCssAsset("toast-styles", toastStyles, 100));
            }
            else if (location == AssetLocation.Footer)
            {
                // jQuery (Footer - first, highest priority)
                assets.Add(CreateJavaScriptAsset("jquery-js", "/assets/libs/jquery/dist/jquery.min.js", 10, AssetLocation.Footer));

                // Bootstrap Bundle JS (Footer - after jQuery)
                var bootstrapAsset = CreateJavaScriptAsset("bootstrap-bundle-js", "/assets/libs/bootstrap/js/bootstrap.bundle.min.js", 20, AssetLocation.Footer);
                bootstrapAsset.Dependencies.Add("jquery-js");
                assets.Add(bootstrapAsset);

                // Simplebar JS (Footer - after Bootstrap)
                var simplebarAsset = CreateJavaScriptAsset("simplebar-js", "/assets/libs/simplebar/simplebar.min.js", 30, AssetLocation.Footer);
                simplebarAsset.Dependencies.Add("jquery-js");
                assets.Add(simplebarAsset);

                // Node Waves JS (Footer - after Bootstrap)
                var wavesAsset = CreateJavaScriptAsset("node-waves-js", "/assets/libs/node-waves/waves.min.js", 40, AssetLocation.Footer);
                wavesAsset.Dependencies.Add("jquery-js");
                assets.Add(wavesAsset);

                // Feather Icons JS (Footer - after Bootstrap)
                var featherAsset = CreateJavaScriptAsset("feather-icons-js", "/assets/libs/feather-icons/feather.min.js", 50, AssetLocation.Footer);
                featherAsset.Dependencies.Add("jquery-js");
                assets.Add(featherAsset);

                // Lord Icon JS (Footer - after Bootstrap)
                var lordIconAsset = CreateJavaScriptAsset("lord-icon-js", "/assets/js/pages/plugins/lord-icon-2.1.0.js", 60, AssetLocation.Footer);
                lordIconAsset.Dependencies.Add("jquery-js");
                assets.Add(lordIconAsset);

                // jQuery Validation (Footer - after jQuery)
                var jqueryValidationAsset = CreateJavaScriptAsset("jquery-validation-js", "/lib/jquery-validation/dist/jquery.validate.min.js", 65, AssetLocation.Footer);
                jqueryValidationAsset.Dependencies.Add("jquery-js");
                assets.Add(jqueryValidationAsset);

                // jQuery Validation Unobtrusive (Footer - after jQuery Validation)
                var jqueryValidationUnobtrusiveAsset = CreateJavaScriptAsset("jquery-validation-unobtrusive-js", "/lib/jquery-validation-unobtrusive/jquery.validate.unobtrusive.min.js", 66, AssetLocation.Footer);
                jqueryValidationUnobtrusiveAsset.Dependencies.AddRange(new[] { "jquery-js", "jquery-validation-js" });
                assets.Add(jqueryValidationUnobtrusiveAsset);

                // Password Addon JS (Footer - for authentication screens)
                var passwordAddonAsset = CreateJavaScriptAsset("password-addon-js", "/assets/js/pages/password-addon.init.js", 67, AssetLocation.Footer);
                passwordAddonAsset.Dependencies.Add("jquery-js");
                assets.Add(passwordAddonAsset);

                // Plugins JS (Footer - last, depends on all others)
                var pluginsAsset = CreateJavaScriptAsset("plugins-js", "/assets/js/plugins.js", 70, AssetLocation.Footer);
                pluginsAsset.Dependencies.AddRange(new[] { "jquery-js", "bootstrap-bundle-js", "simplebar-js", "waves-js", "feather-icons-js", "lord-icon-js" });
                assets.Add(pluginsAsset);
                
                var appAsset = CreateJavaScriptAsset("app-js", "/assets/js/app.js", 80, AssetLocation.Footer);
                appAsset.Dependencies.AddRange(new[] { "jquery-js", "bootstrap-bundle-js", "simplebar-js", "waves-js", "feather-icons-js", "lord-icon-js" });
                assets.Add(appAsset);
            }
            
            return Task.FromResult<IEnumerable<Asset>>(assets);
        }
    }
} 