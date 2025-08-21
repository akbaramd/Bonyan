using Bonyan.AspNetCore.ZoneComponent;

namespace Bonyan.Novino.Module.UserManagement.Abstractions.Zones.IndexPage;

/// <summary>
/// Zone model for user index page table header area
/// Allows other modules to add custom table columns
/// </summary>
public class UserIndexPageTableHeaderZone : IZone
{
    /// <summary>
    /// Whether the current user can create new users
    /// </summary>
    public bool CanCreate { get; set; }

    /// <summary>
    /// Whether the current user can edit users
    /// </summary>
    public bool CanEdit { get; set; }

    /// <summary>
    /// Whether the current user can delete users
    /// </summary>
    public bool CanDelete { get; set; }

    /// <summary>
    /// Whether the current user can export users
    /// </summary>
    public bool CanExport { get; set; }

    /// <summary>
    /// Whether the current user can perform bulk operations
    /// </summary>
    public bool CanBulkDelete { get; set; }

    /// <summary>
    /// Whether the current user can bulk activate users
    /// </summary>
    public bool CanBulkActivate { get; set; }

    /// <summary>
    /// Whether the current user can bulk deactivate users
    /// </summary>
    public bool CanBulkDeactivate { get; set; }

    /// <summary>
    /// Custom table header columns from other modules
    /// </summary>
    public List<string> CustomHeaderColumns { get; set; } = new();

    /// <summary>
    /// Custom table header actions from other modules
    /// </summary>
    public List<string> CustomHeaderActions { get; set; } = new();

    /// <summary>
    /// Additional header data that can be used by other modules
    /// </summary>
    public Dictionary<string, object> AdditionalData { get; set; } = new();

    /// <summary>
    /// Add a custom header column
    /// </summary>
    /// <param name="columnHtml">The HTML for the header column</param>
    public void AddCustomHeaderColumn(string columnHtml)
    {
        if (!string.IsNullOrWhiteSpace(columnHtml))
        {
            CustomHeaderColumns.Add(columnHtml);
        }
    }

    /// <summary>
    /// Add a custom header action
    /// </summary>
    /// <param name="actionHtml">The HTML for the header action</param>
    public void AddCustomHeaderAction(string actionHtml)
    {
        if (!string.IsNullOrWhiteSpace(actionHtml))
        {
            CustomHeaderActions.Add(actionHtml);
        }
    }

    /// <summary>
    /// Gets the unique zone identifier
    /// </summary>
    public string ZoneId => "user-index-page-table-header-zone";

    /// <summary>
    /// Gets the zone name for rendering
    /// </summary>
    public string ZoneName => "user-index-page-table-header";

    /// <summary>
    /// Gets additional metadata for the zone
    /// </summary>
    public IDictionary<string, object>? Metadata => AdditionalData;
} 