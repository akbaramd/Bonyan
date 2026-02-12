namespace Bonyan.DependencyInjection;

/// <summary>
/// When applied to a type, disables specific Bonyan features (e.g. interceptors) for that type.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class DisableBonFeaturesAttribute : Attribute
{
    /// <summary>
    /// When true, interceptors are not applied to this type.
    /// </summary>
    public bool DisableInterceptors { get; set; }
}
