using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Assets.TagHelpers
{
    /// <summary>
    /// Tag helper for rendering assets at specific locations
    /// Usage: &lt;assets location="Head" /&gt; or &lt;assets location="Footer" type="JavaScript" /&gt;
    /// </summary>
    [HtmlTargetElement("assets")]
    public class AssetTagHelper : TagHelper
    {
        private readonly IAssetManager _assetManager;

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; } = null!;

        /// <summary>
        /// The location where assets should be rendered
        /// </summary>
        [HtmlAttributeName("location")]
        public AssetLocation Location { get; set; } = AssetLocation.Head;

        /// <summary>
        /// Optional: Filter assets by type (Css, JavaScript, etc.)
        /// </summary>
        [HtmlAttributeName("type")]
        public AssetType? Type { get; set; }

        /// <summary>
        /// Whether to render assets asynchronously (default: true)
        /// </summary>
        [HtmlAttributeName("async")]
        public bool Async { get; set; } = true;

        public AssetTagHelper(IAssetManager assetManager)
        {
            _assetManager = assetManager;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            // Remove the tag itself from output
            output.TagName = null;

            try
            {
                var user = ViewContext.HttpContext.User;
                string html;

                if (Type.HasValue)
                {
                    // Render specific asset type
                    if (Async)
                    {
                        html = await _assetManager.RenderAssetsByTypeAsync(Location, Type.Value, user);
                    }
                    else
                    {
                        html = _assetManager.RenderAssetsByType(Location, Type.Value, user);
                    }
                }
                else
                {
                    // Render all assets for the location
                    if (Async)
                    {
                        html = await _assetManager.RenderAssetsAsync(Location, user);
                    }
                    else
                    {
                        html = _assetManager.RenderAssets(Location, user);
                    }
                }

                if (!string.IsNullOrEmpty(html))
                {
                    output.Content.SetHtmlContent(html);
                }
            }
            catch (Exception ex)
            {
                // Log error and render empty to prevent breaking the page
                var logger = ViewContext.HttpContext.RequestServices.GetService<Microsoft.Extensions.Logging.ILogger<AssetTagHelper>>();
                logger?.LogError(ex, "Error rendering assets for location '{Location}'", Location);
                
                output.Content.SetContent($"<!-- Error rendering assets for location '{Location}' -->");
            }
        }
    }

    /// <summary>
    /// Tag helper for rendering CSS assets
    /// Usage: &lt;css-assets location="Head" /&gt;
    /// </summary>
    [HtmlTargetElement("css-assets")]
    public class CssAssetTagHelper : TagHelper
    {
        private readonly IAssetManager _assetManager;

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; } = null!;

        /// <summary>
        /// The location where CSS assets should be rendered
        /// </summary>
        [HtmlAttributeName("location")]
        public AssetLocation Location { get; set; } = AssetLocation.Head;

        public CssAssetTagHelper(IAssetManager assetManager)
        {
            _assetManager = assetManager;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = null;

            try
            {
                var user = ViewContext.HttpContext.User;
                var html = await _assetManager.RenderAssetsByTypeAsync(Location, AssetType.Css, user);

                if (!string.IsNullOrEmpty(html))
                {
                    output.Content.SetHtmlContent(html);
                }
            }
            catch (Exception ex)
            {
                var logger = ViewContext.HttpContext.RequestServices.GetService<Microsoft.Extensions.Logging.ILogger<CssAssetTagHelper>>();
                logger?.LogError(ex, "Error rendering CSS assets for location '{Location}'", Location);
                
                output.Content.SetContent($"<!-- Error rendering CSS assets for location '{Location}' -->");
            }
        }
    }

    /// <summary>
    /// Tag helper for rendering JavaScript assets
    /// Usage: &lt;js-assets location="Footer" /&gt;
    /// </summary>
    [HtmlTargetElement("js-assets")]
    public class JavaScriptAssetTagHelper : TagHelper
    {
        private readonly IAssetManager _assetManager;

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; } = null!;

        /// <summary>
        /// The location where JavaScript assets should be rendered
        /// </summary>
        [HtmlAttributeName("location")]
        public AssetLocation Location { get; set; } = AssetLocation.Footer;

        public JavaScriptAssetTagHelper(IAssetManager assetManager)
        {
            _assetManager = assetManager;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = null;

            try
            {
                var user = ViewContext.HttpContext.User;
                var html = await _assetManager.RenderAssetsByTypeAsync(Location, AssetType.JavaScript, user);

                if (!string.IsNullOrEmpty(html))
                {
                    output.Content.SetHtmlContent(html);
                }
            }
            catch (Exception ex)
            {
                var logger = ViewContext.HttpContext.RequestServices.GetService<Microsoft.Extensions.Logging.ILogger<JavaScriptAssetTagHelper>>();
                logger?.LogError(ex, "Error rendering JavaScript assets for location '{Location}'", Location);
                
                output.Content.SetContent($"<!-- Error rendering JavaScript assets for location '{Location}' -->");
            }
        }
    }

    /// <summary>
    /// Tag helper for rendering inline CSS assets
    /// Usage: &lt;inline-css location="InlineHead" /&gt;
    /// </summary>
    [HtmlTargetElement("inline-css")]
    public class InlineCssAssetTagHelper : TagHelper
    {
        private readonly IAssetManager _assetManager;

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; } = null!;

        /// <summary>
        /// The location where inline CSS assets should be rendered
        /// </summary>
        [HtmlAttributeName("location")]
        public AssetLocation Location { get; set; } = AssetLocation.InlineHead;

        public InlineCssAssetTagHelper(IAssetManager assetManager)
        {
            _assetManager = assetManager;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = null;

            try
            {
                var user = ViewContext.HttpContext.User;
                var html = await _assetManager.RenderAssetsByTypeAsync(Location, AssetType.InlineCss, user);

                if (!string.IsNullOrEmpty(html))
                {
                    output.Content.SetHtmlContent(html);
                }
            }
            catch (Exception ex)
            {
                var logger = ViewContext.HttpContext.RequestServices.GetService<Microsoft.Extensions.Logging.ILogger<InlineCssAssetTagHelper>>();
                logger?.LogError(ex, "Error rendering inline CSS assets for location '{Location}'", Location);
                
                output.Content.SetContent($"<!-- Error rendering inline CSS assets for location '{Location}' -->");
            }
        }
    }

    /// <summary>
    /// Tag helper for rendering inline JavaScript assets
    /// Usage: &lt;inline-js location="InlineFooter" /&gt;
    /// </summary>
    [HtmlTargetElement("inline-js")]
    public class InlineJavaScriptAssetTagHelper : TagHelper
    {
        private readonly IAssetManager _assetManager;

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; } = null!;

        /// <summary>
        /// The location where inline JavaScript assets should be rendered
        /// </summary>
        [HtmlAttributeName("location")]
        public AssetLocation Location { get; set; } = AssetLocation.InlineFooter;

        public InlineJavaScriptAssetTagHelper(IAssetManager assetManager)
        {
            _assetManager = assetManager;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = null;

            try
            {
                var user = ViewContext.HttpContext.User;
                var html = await _assetManager.RenderAssetsByTypeAsync(Location, AssetType.InlineJavaScript, user);

                if (!string.IsNullOrEmpty(html))
                {
                    output.Content.SetHtmlContent(html);
                }
            }
            catch (Exception ex)
            {
                var logger = ViewContext.HttpContext.RequestServices.GetService<Microsoft.Extensions.Logging.ILogger<InlineJavaScriptAssetTagHelper>>();
                logger?.LogError(ex, "Error rendering inline JavaScript assets for location '{Location}'", Location);
                
                output.Content.SetContent($"<!-- Error rendering inline JavaScript assets for location '{Location}' -->");
            }
        }
    }
} 