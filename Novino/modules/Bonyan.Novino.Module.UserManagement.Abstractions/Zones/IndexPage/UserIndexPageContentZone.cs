using Bonyan.AspNetCore.ZoneComponent;

namespace Bonyan.Novino.Module.UserManagement.Abstractions.Zones.IndexPage;

/// <summary>
/// Zone model for user index page content area
/// Allows other modules to add custom content
/// </summary>
public class UserIndexPageContentZone : IZone
{
    /// <summary>
    /// The main view model
    /// </summary>
    public object? ViewModel { get; set; }

    /// <summary>
    /// Custom content from other modules
    /// </summary>
    public List<string> CustomContent { get; set; } = new();

    /// <summary>
    /// Custom scripts from other modules
    /// </summary>
    public List<string> CustomScripts { get; set; } = new();

    /// <summary>
    /// Custom styles from other modules
    /// </summary>
    public List<string> CustomStyles { get; set; } = new();

    /// <summary>
    /// Additional content data that can be used by other modules
    /// </summary>
    public Dictionary<string, object> AdditionalData { get; set; } = new();

    /// <summary>
    /// Add custom content
    /// </summary>
    /// <param name="contentHtml">The HTML for the custom content</param>
    public void AddCustomContent(string contentHtml)
    {
        if (!string.IsNullOrWhiteSpace(contentHtml))
        {
            CustomContent.Add(contentHtml);
        }
    }

    /// <summary>
    /// Add custom script
    /// </summary>
    /// <param name="scriptHtml">The HTML for the custom script</param>
    public void AddCustomScript(string scriptHtml)
    {
        if (!string.IsNullOrWhiteSpace(scriptHtml))
        {
            CustomScripts.Add(scriptHtml);
        }
    }

    /// <summary>
    /// Add custom style
    /// </summary>
    /// <param name="styleHtml">The HTML for the custom style</param>
    public void AddCustomStyle(string styleHtml)
    {
        if (!string.IsNullOrWhiteSpace(styleHtml))
        {
            CustomStyles.Add(styleHtml);
        }
    }

    /// <summary>
    /// Gets the unique zone identifier
    /// </summary>
    public string ZoneId => "user-index-page-content-zone";

    /// <summary>
    /// Gets the zone name for rendering
    /// </summary>
    public string ZoneName => "user-index-page-content";

    /// <summary>
    /// Gets additional metadata for the zone
    /// </summary>
    public IDictionary<string, object>? Metadata => AdditionalData;
} 