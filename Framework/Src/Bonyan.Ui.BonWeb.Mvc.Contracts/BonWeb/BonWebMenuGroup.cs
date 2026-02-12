namespace Bonyan.Ui.BonWeb.Mvc.Contracts;

/// <summary>
/// A group of sidebar menu items. When <see cref="GroupName"/> is set, show a group header; otherwise render items without a header.
/// </summary>
public class BonWebMenuGroup
{
    /// <summary>Display name for the group header (can be localized by the provider). When null or empty, no group header is shown.</summary>
    public string? GroupName { get; set; }

    /// <summary>Menu items in this group (can have nested Children).</summary>
    public List<BonWebMenuItem> Items { get; set; } = new();
}
