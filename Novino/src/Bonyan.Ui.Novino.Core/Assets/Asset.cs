namespace Bonyan.Novino.Core.Assets
{
    /// <summary>
    /// Base class for all assets (CSS, JavaScript, etc.)
    /// </summary>
    public abstract class Asset
    {
        /// <summary>
        /// Gets or sets the unique identifier for this asset
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Gets or sets the name of the asset
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the type of asset
        /// </summary>
        public abstract AssetType Type { get; }

        /// <summary>
        /// Gets or sets where this asset should be rendered
        /// </summary>
        public AssetLocation Location { get; set; } = AssetLocation.Head;

        /// <summary>
        /// Gets or sets the loading priority (lower values = higher priority)
        /// </summary>
        public int Priority { get; set; } = 100;

        /// <summary>
        /// Gets or sets whether this asset is enabled
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets the loading strategy for this asset
        /// </summary>
        public AssetLoadingStrategy LoadingStrategy { get; set; } = AssetLoadingStrategy.Normal;

        /// <summary>
        /// Gets or sets the dependencies (other asset IDs that must be loaded first)
        /// </summary>
        public List<string> Dependencies { get; set; } = new();

        /// <summary>
        /// Gets or sets the condition for including this asset (e.g., browser, device, etc.)
        /// </summary>
        public string? Condition { get; set; }

        /// <summary>
        /// Gets or sets additional attributes for the HTML element
        /// </summary>
        public Dictionary<string, string> Attributes { get; set; } = new();

        /// <summary>
        /// Gets or sets metadata for this asset
        /// </summary>
        public Dictionary<string, object> Metadata { get; set; } = new();

        /// <summary>
        /// Gets or sets the module or provider that contributed this asset
        /// </summary>
        public string? ProviderId { get; set; }

        /// <summary>
        /// Gets or sets the version of this asset (for cache busting)
        /// </summary>
        public string? Version { get; set; }

        /// <summary>
        /// Renders the asset as HTML
        /// </summary>
        /// <returns>HTML string for this asset</returns>
        public abstract string Render();

        protected Asset(string name)
        {
            Name = name;
        }

        public override bool Equals(object? obj)
        {
            if (obj is Asset other)
            {
                return Id.Equals(other.Id, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode(StringComparison.OrdinalIgnoreCase);
        }

        public override string ToString()
        {
            return $"{Type}: {Name} ({Location})";
        }
    }

    /// <summary>
    /// Represents a CSS stylesheet asset
    /// </summary>
    public class CssAsset : Asset
    {
        /// <summary>
        /// Gets the type of asset
        /// </summary>
        public override AssetType Type => AssetType.Css;

        /// <summary>
        /// Gets or sets the URL or path to the CSS file
        /// </summary>
        public string Href { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the media type (e.g., "screen", "print", "all")
        /// </summary>
        public string Media { get; set; } = "all";

        /// <summary>
        /// Gets or sets the integrity hash for security
        /// </summary>
        public string? Integrity { get; set; }

        /// <summary>
        /// Gets or sets the crossorigin attribute
        /// </summary>
        public string? CrossOrigin { get; set; }

        public CssAsset(string name, string href) : base(name)
        {
            Href = href;
            Location = AssetLocation.Head; // CSS should always go in head
        }

        public override string Render()
        {
            var attributes = new List<string>
            {
                $"rel=\"stylesheet\"",
                $"href=\"{Href}\""
            };

            if (!string.IsNullOrEmpty(Media))
                attributes.Add($"media=\"{Media}\"");

            if (!string.IsNullOrEmpty(Integrity))
                attributes.Add($"integrity=\"{Integrity}\"");

            if (!string.IsNullOrEmpty(CrossOrigin))
                attributes.Add($"crossorigin=\"{CrossOrigin}\"");

            // Add custom attributes
            foreach (var attr in Attributes)
            {
                attributes.Add($"{attr.Key}=\"{attr.Value}\"");
            }

            var linkTag = $"<link {string.Join(" ", attributes)} />";

            // Wrap in condition if specified
            if (!string.IsNullOrEmpty(Condition))
            {
                return $"<!--[if {Condition}]>{linkTag}<![endif]-->";
            }

            return linkTag;
        }
    }

    /// <summary>
    /// Represents a JavaScript asset
    /// </summary>
    public class JavaScriptAsset : Asset
    {
        /// <summary>
        /// Gets the type of asset
        /// </summary>
        public override AssetType Type => AssetType.JavaScript;

        /// <summary>
        /// Gets or sets the URL or path to the JavaScript file
        /// </summary>
        public string Src { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the integrity hash for security
        /// </summary>
        public string? Integrity { get; set; }

        /// <summary>
        /// Gets or sets the crossorigin attribute
        /// </summary>
        public string? CrossOrigin { get; set; }

        /// <summary>
        /// Gets or sets the module type (e.g., "module", "nomodule")
        /// </summary>
        public string? ModuleType { get; set; }

        public JavaScriptAsset(string name, string src) : base(name)
        {
            Src = src;
            Location = AssetLocation.Footer; // JavaScript typically goes in footer
        }

        public override string Render()
        {
            var attributes = new List<string>
            {
                $"src=\"{Src}\""
            };

            if (LoadingStrategy == AssetLoadingStrategy.Async)
                attributes.Add("async");
            else if (LoadingStrategy == AssetLoadingStrategy.Defer)
                attributes.Add("defer");

            if (!string.IsNullOrEmpty(Integrity))
                attributes.Add($"integrity=\"{Integrity}\"");

            if (!string.IsNullOrEmpty(CrossOrigin))
                attributes.Add($"crossorigin=\"{CrossOrigin}\"");

            if (!string.IsNullOrEmpty(ModuleType))
                attributes.Add($"type=\"{ModuleType}\"");

            // Add custom attributes
            foreach (var attr in Attributes)
            {
                attributes.Add($"{attr.Key}=\"{attr.Value}\"");
            }

            var scriptTag = $"<script {string.Join(" ", attributes)}></script>";

            // Wrap in condition if specified
            if (!string.IsNullOrEmpty(Condition))
            {
                return $"<!--[if {Condition}]>{scriptTag}<![endif]-->";
            }

            return scriptTag;
        }
    }

    /// <summary>
    /// Represents inline CSS styles
    /// </summary>
    public class InlineCssAsset : Asset
    {
        /// <summary>
        /// Gets the type of asset
        /// </summary>
        public override AssetType Type => AssetType.InlineCss;

        /// <summary>
        /// Gets or sets the CSS content
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the media type
        /// </summary>
        public string Media { get; set; } = "all";

        public InlineCssAsset(string name, string content) : base(name)
        {
            Content = content;
            Location = AssetLocation.InlineHead;
        }

        public override string Render()
        {
            var attributes = new List<string>();

            if (!string.IsNullOrEmpty(Media))
                attributes.Add($"media=\"{Media}\"");

            // Add custom attributes
            foreach (var attr in Attributes)
            {
                attributes.Add($"{attr.Key}=\"{attr.Value}\"");
            }

            var attributeString = attributes.Any() ? " " + string.Join(" ", attributes) : "";
            return $"<style{attributeString}>\n{Content}\n</style>";
        }
    }

    /// <summary>
    /// Represents inline JavaScript code
    /// </summary>
    public class InlineJavaScriptAsset : Asset
    {
        /// <summary>
        /// Gets the type of asset
        /// </summary>
        public override AssetType Type => AssetType.InlineJavaScript;

        /// <summary>
        /// Gets or sets the JavaScript content
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the module type
        /// </summary>
        public string? ModuleType { get; set; }

        public InlineJavaScriptAsset(string name, string content) : base(name)
        {
            Content = content;
            Location = AssetLocation.InlineFooter;
        }

        public override string Render()
        {
            var attributes = new List<string>();

            if (!string.IsNullOrEmpty(ModuleType))
                attributes.Add($"type=\"{ModuleType}\"");

            // Add custom attributes
            foreach (var attr in Attributes)
            {
                attributes.Add($"{attr.Key}=\"{attr.Value}\"");
            }

            var attributeString = attributes.Any() ? " " + string.Join(" ", attributes) : "";
            return $"<script{attributeString}>\n{Content}\n</script>";
        }
    }

    /// <summary>
    /// Represents custom HTML content
    /// </summary>
    public class HtmlAsset : Asset
    {
        /// <summary>
        /// Gets the type of asset
        /// </summary>
        public override AssetType Type => AssetType.Html;

        /// <summary>
        /// Gets or sets the HTML content
        /// </summary>
        public string Content { get; set; } = string.Empty;

        public HtmlAsset(string name, string content) : base(name)
        {
            Content = content;
        }

        public override string Render()
        {
            return Content;
        }
    }
} 