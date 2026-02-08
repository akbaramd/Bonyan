namespace WebApi.Demo.Demos.Models;

/// <summary>
/// Demo item model for testing attributed services.
/// </summary>
public class DemoItem
{
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
