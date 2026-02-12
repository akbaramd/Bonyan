namespace Bonyan.Ui.BonWeb.Mvc;

/// <summary>
/// Application display info bound from appsettings "Application" section.
/// Used in layout for sidebar header, brand name, and page title.
/// </summary>
public class BonWebApplicationInfo
{
    /// <summary>
    /// Display name of the application (e.g. "Dashboard", "My App").
    /// When not set, the layout falls back to the localized "Layout:BonWeb" resource.
    /// </summary>
    public string? Name { get; set; }
}
