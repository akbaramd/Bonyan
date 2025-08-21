using Bonyan.AspNetCore.ZoneComponent;

namespace Bonyan.Novino.Module.UserManagement.Abstractions.Zones.IndexPage;

/// <summary>
/// Zone model for user index page table filter area
/// Allows other modules to add custom filter controls
/// </summary>
public class UserIndexPageTableFilterZone : IZone
{
    /// <summary>
    /// The current search term
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// The current status filter
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// The current sort by field
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// The current sort order
    /// </summary>
    public string? SortOrder { get; set; }

    /// <summary>
    /// Whether any filters are currently applied
    /// </summary>
    public bool HasFilters { get; set; }

    /// <summary>
    /// Additional filter data that can be used by other modules
    /// </summary>
    public Dictionary<string, object> AdditionalFilters { get; set; } = new();

    /// <summary>
    /// Whether the filter panel is expanded
    /// </summary>
    public bool IsExpanded { get; set; }

    /// <summary>
    /// Custom filter controls HTML from other modules
    /// </summary>
    public List<string> CustomFilterControls { get; set; } = new();

    /// <summary>
    /// Add a custom filter control
    /// </summary>
    /// <param name="controlHtml">The HTML for the filter control</param>
    public void AddCustomFilterControl(string controlHtml)
    {
        if (!string.IsNullOrWhiteSpace(controlHtml))
        {
            CustomFilterControls.Add(controlHtml);
        }
    }

    /// <summary>
    /// Gets the unique zone identifier
    /// </summary>
    public string ZoneId => "user-index-page-table-filter-zone";

    /// <summary>
    /// Gets the zone name for rendering
    /// </summary>
    public string ZoneName => "user-index-page-table-filter";

    /// <summary>
    /// Gets additional metadata for the zone
    /// </summary>
    public IDictionary<string, object>? Metadata => AdditionalFilters;
} 