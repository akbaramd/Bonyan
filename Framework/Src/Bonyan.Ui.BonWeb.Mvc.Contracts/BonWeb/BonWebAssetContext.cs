namespace Bonyan.Ui.BonWeb.Mvc.Contracts;

/// <summary>
/// Context passed when resolving assets so providers can add RTL-specific or culture-specific assets.
/// Developers can check <see cref="IsRtl"/> to add RTL versions (e.g. bootstrap.rtl.min.css).
/// </summary>
public class BonWebAssetContext
{
    /// <summary>
    /// Whether the current request is right-to-left (e.g. Arabic, Persian).
    /// Use this to add RTL stylesheets or scripts.
    /// </summary>
    public bool IsRtl { get; set; }

    /// <summary>
    /// Current culture name (e.g. "en-US", "fa-IR"). Use for culture-specific assets if needed.
    /// </summary>
    public string CultureName { get; set; } = string.Empty;

    /// <summary>
    /// Current language two-letter code (e.g. "en", "fa").
    /// </summary>
    public string LanguageCode { get; set; } = string.Empty;
}
