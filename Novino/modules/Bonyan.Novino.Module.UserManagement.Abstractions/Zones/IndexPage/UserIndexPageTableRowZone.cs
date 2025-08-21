using Bonyan.AspNetCore.ZoneComponent;

namespace Bonyan.Novino.Module.UserManagement.Abstractions.Zones.IndexPage;

/// <summary>
/// Zone model for user index page table row area
/// Contains only essential data - components should fetch their own data
/// Allows other modules to add custom table row data
/// </summary>
public class UserIndexPageTableRowZone : IZone
{
    public string ZoneId => "user_index_page_table_row";
    public string ZoneName => "UserManagementUserIndexPageTableRow";
    public IDictionary<string, object>? Metadata { get; set; } = new Dictionary<string, object>();

    /// <summary>
    /// User ID - the only essential data needed by row components
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Row index for positioning
    /// </summary>
    public int RowIndex { get; set; }

    /// <summary>
    /// Add custom row column HTML to be rendered
    /// </summary>
    /// <param name="columnHtml">The HTML for the row column</param>
    public void AddCustomRowColumn(string columnHtml)
    {
        if (Metadata == null)
            Metadata = new Dictionary<string, object>();

        if (!Metadata.ContainsKey("CustomRowColumns"))
            Metadata["CustomRowColumns"] = new List<string>();

        ((List<string>)Metadata["CustomRowColumns"]).Add(columnHtml);
    }

    /// <summary>
    /// Add custom row action HTML to be rendered
    /// </summary>
    /// <param name="actionHtml">The HTML for the row action</param>
    public void AddCustomRowAction(string actionHtml)
    {
        if (Metadata == null)
            Metadata = new Dictionary<string, object>();

        if (!Metadata.ContainsKey("CustomRowActions"))
            Metadata["CustomRowActions"] = new List<string>();

        ((List<string>)Metadata["CustomRowActions"]).Add(actionHtml);
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