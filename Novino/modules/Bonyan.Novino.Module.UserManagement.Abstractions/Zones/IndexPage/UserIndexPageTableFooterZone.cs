using Bonyan.AspNetCore.ZoneComponent;

namespace Bonyan.Novino.Module.UserManagement.Abstractions.Zones.IndexPage;

/// <summary>
/// Zone model for user index page table footer area
/// Allows other modules to add custom footer content
/// </summary>
public class UserIndexPageTableFooterZone : IZone
{
    /// <summary>
    /// The current page number
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// The total number of pages
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// The page size
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// The total count of items
    /// </summary>
    public int TotalCount { get; set; }



    /// <summary>
    /// Whether filters are currently applied
    /// </summary>
    public bool HasFilters { get; set; }

    /// <summary>
    /// Whether the user can export data
    /// </summary>
    public bool CanExport { get; set; }

    /// <summary>
    /// Whether the user can perform bulk delete operations
    /// </summary>
    public bool CanBulkDelete { get; set; }

    /// <summary>
    /// Whether the user can perform bulk activate operations
    /// </summary>
    public bool CanBulkActivate { get; set; }

    /// <summary>
    /// Whether the user can perform bulk deactivate operations
    /// </summary>
    public bool CanBulkDeactivate { get; set; }

    /// <summary>
    /// Custom footer content from other modules
    /// </summary>
    public List<string> CustomFooterContent { get; set; } = new();

    /// <summary>
    /// Custom footer actions from other modules
    /// </summary>
    public List<string> CustomFooterActions { get; set; } = new();

    /// <summary>
    /// Additional footer data that can be used by other modules
    /// </summary>
    public Dictionary<string, object> AdditionalData { get; set; } = new();

    /// <summary>
    /// Add custom footer content
    /// </summary>
    /// <param name="contentHtml">The HTML for the footer content</param>
    public void AddCustomFooterContent(string contentHtml)
    {
        if (!string.IsNullOrWhiteSpace(contentHtml))
        {
            CustomFooterContent.Add(contentHtml);
        }
    }

    /// <summary>
    /// Add a custom footer action
    /// </summary>
    /// <param name="actionHtml">The HTML for the footer action</param>
    public void AddCustomFooterAction(string actionHtml)
    {
        if (!string.IsNullOrWhiteSpace(actionHtml))
        {
            CustomFooterActions.Add(actionHtml);
        }
    }

    /// <summary>
    /// Gets the start index of the current page
    /// </summary>
    public int StartIndex => (CurrentPage - 1) * PageSize + 1;

    /// <summary>
    /// Gets the end index of the current page
    /// </summary>
    public int EndIndex => Math.Min(CurrentPage * PageSize, TotalCount);

    /// <summary>
    /// Gets whether there is a previous page
    /// </summary>
    public bool HasPreviousPage => CurrentPage > 1;

    /// <summary>
    /// Gets whether there is a next page
    /// </summary>
    public bool HasNextPage => CurrentPage < TotalPages;

    /// <summary>
    /// Gets the display text for the current page range
    /// </summary>
    public string GetPageRangeText()
    {
        if (HasFilters)
        {
            return $"نمایش {StartIndex} تا {EndIndex} از {TotalCount} نتیجه";
        }
        return $"نمایش {StartIndex} تا {EndIndex} از {TotalCount} کاربر";
    }

    /// <summary>
    /// Gets the start page for pagination display
    /// </summary>
    public int GetStartPage()
    {
        return Math.Max(1, CurrentPage - 2);
    }

    /// <summary>
    /// Gets the end page for pagination display
    /// </summary>
    public int GetEndPage()
    {
        return Math.Min(TotalPages, CurrentPage + 2);
    }

    /// <summary>
    /// Gets the unique zone identifier
    /// </summary>
    public string ZoneId => "user-index-page-table-footer-zone";

    /// <summary>
    /// Gets the zone name for rendering
    /// </summary>
    public string ZoneName => "user-index-page-table-footer";

    /// <summary>
    /// Gets additional metadata for the zone
    /// </summary>
    public IDictionary<string, object>? Metadata => AdditionalData;
} 