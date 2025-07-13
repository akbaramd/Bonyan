namespace Assets
{
    /// <summary>
    /// Defines where assets should be rendered in the HTML document
    /// </summary>
    public enum AssetLocation
    {
        /// <summary>
        /// Render in the HTML head section (recommended for CSS and critical JS)
        /// </summary>
        Head = 0,

        /// <summary>
        /// Render before closing body tag (recommended for non-critical JS)
        /// </summary>
        Footer = 1,

        /// <summary>
        /// Render after jQuery is loaded (for jQuery-dependent scripts)
        /// </summary>
        AfterJQuery = 2,

        /// <summary>
        /// Render after Bootstrap is loaded (for Bootstrap-dependent scripts)
        /// </summary>
        AfterBootstrap = 3,

        /// <summary>
        /// Render inline in the head section
        /// </summary>
        InlineHead = 4,

        /// <summary>
        /// Render inline before closing body tag
        /// </summary>
        InlineFooter = 5,

        /// <summary>
        /// Render in a custom location defined by the theme or layout
        /// </summary>
        Custom = 99
    }

    /// <summary>
    /// Defines the type of asset
    /// </summary>
    public enum AssetType
    {
        /// <summary>
        /// CSS stylesheet file
        /// </summary>
        Css = 0,

        /// <summary>
        /// JavaScript file
        /// </summary>
        JavaScript = 1,

        /// <summary>
        /// Inline CSS styles
        /// </summary>
        InlineCss = 2,

        /// <summary>
        /// Inline JavaScript code
        /// </summary>
        InlineJavaScript = 3,

        /// <summary>
        /// Custom HTML content
        /// </summary>
        Html = 4
    }

    /// <summary>
    /// Defines the loading strategy for assets
    /// </summary>
    public enum AssetLoadingStrategy
    {
        /// <summary>
        /// Normal loading (blocking)
        /// </summary>
        Normal = 0,

        /// <summary>
        /// Async loading (non-blocking)
        /// </summary>
        Async = 1,

        /// <summary>
        /// Defer loading (execute after document parsing)
        /// </summary>
        Defer = 2,

        /// <summary>
        /// Preload (high priority resource)
        /// </summary>
        Preload = 3,

        /// <summary>
        /// Prefetch (low priority resource for future navigation)
        /// </summary>
        Prefetch = 4
    }
} 