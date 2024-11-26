// ListItemModel.cs

namespace Bonyan.Ui.Blazimum.Components.List;

public class ListItemModel
{
    public required string Key { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsExpanded { get; set; } = false; // Default collapsed
    public List<ListItemModel> Children { get; set; } = new();
    public Dictionary<string, object> Meta { get; set; } = new(); // Metadata dictionary
}
