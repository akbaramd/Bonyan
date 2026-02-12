namespace Bonyan.Ui.BonWeb.Mvc.Contracts;

/// <summary>
/// Defines an asset (CSS or JS) for the BonWeb dashboard.
/// </summary>
public class BonWebAssetDefinition
{
    public string Name { get; set; } = string.Empty;
    public BonWebAssetType Type { get; set; }
    public string Source { get; set; } = string.Empty; // URL or path
    public int Priority { get; set; } = 100;

    public BonWebAssetDefinition() { }

    public BonWebAssetDefinition(string name, BonWebAssetType type, string source, int priority = 100)
    {
        Name = name;
        Type = type;
        Source = source;
        Priority = priority;
    }
}

/// <summary>
/// Type of asset.
/// </summary>
public enum BonWebAssetType
{
    Css,
    Script
}
