using Bonyan.AspNetCore.ZoneComponent;

namespace Bonyan.Novino.Module.UserManagement.Abstractions.Zones.IndexPage;

/// <summary>
/// Zone model for user table actions that contains only essential data
/// Allows other modules to add custom action buttons for each user row
/// Components should fetch their own data based on UserId
/// </summary>
public class UserIndexPageTableActionZone : IZone
{
    public string ZoneId => "user_index_page_table_action";
    public string ZoneName => "UserManagementUserIndexPageTableAction";
    public IDictionary<string, object>? Metadata { get; set; } = new Dictionary<string, object>();

    /// <summary>
    /// User ID - the only essential data needed by action components
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Add custom action HTML to be rendered
    /// </summary>
    /// <param name="actionHtml">The HTML for the custom action</param>
    public void AddCustomAction(string actionHtml)
    {
        if (Metadata == null)
            Metadata = new Dictionary<string, object>();

        if (!Metadata.ContainsKey("CustomActions"))
            Metadata["CustomActions"] = new List<string>();

        ((List<string>)Metadata["CustomActions"]).Add(actionHtml);
    }

    /// <summary>
    /// Add additional data that can be used by components
    /// </summary>
    /// <param name="key">The key for the data</param>
    /// <param name="value">The value to store</param>
    public void AddAdditionalData(string key, object value)
    {
        if (Metadata == null)
            Metadata = new Dictionary<string, object>();

        if (!Metadata.ContainsKey("AdditionalData"))
            Metadata["AdditionalData"] = new Dictionary<string, object>();

        ((Dictionary<string, object>)Metadata["AdditionalData"])[key] = value;
    }
} 