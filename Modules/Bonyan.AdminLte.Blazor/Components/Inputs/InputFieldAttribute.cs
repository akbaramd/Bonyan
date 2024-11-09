namespace Bonyan.AdminLte.Components.Inputs;

public class InputFieldAttribute : Attribute
{
    public string Label { get; set; }
    public string Placeholder { get; set; }
    public string Icon { get; set; }
    public bool IsPassword { get; set; } = false;
}