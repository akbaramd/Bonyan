namespace Bonyan.Modularity;

/// <summary>
/// Declares a dependency on another module using static metadata.
/// This attribute-based approach avoids instantiating modules just to read dependencies,
/// supporting the microkernel architecture goal of keeping plug-ins isolated.
/// </summary>
/// <remarks>
/// Use this attribute to declare module dependencies explicitly:
/// <code>
/// [DependsOn(typeof(IdentityModule))]
/// [DependsOn(typeof(TenantModule))]
/// public class NotificationModule : BonModule { }
/// </code>
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public sealed class DependsOnAttribute : Attribute, IDependedTypesProvider
{
    /// <summary>
    /// Gets the module type that this module depends on.
    /// </summary>
    public Type ModuleType { get; }

    /// <summary>
    /// Gets or sets the reason for this dependency.
    /// Required for plugin-to-plugin dependencies to enforce explicit intent.
    /// </summary>
    public string? Reason { get; init; }

    /// <summary>
    /// Initializes a new instance of <see cref="DependsOnAttribute"/>.
    /// </summary>
    /// <param name="moduleType">The module type to depend on.</param>
    /// <exception cref="ArgumentNullException">Thrown if moduleType is null.</exception>
    public DependsOnAttribute(Type moduleType)
    {
        ModuleType = moduleType ?? throw new ArgumentNullException(nameof(moduleType));
    }

    /// <summary>
    /// Gets the depended types for this attribute.
    /// </summary>
    /// <returns>Array containing the module type this attribute declares a dependency on.</returns>
    public Type[] GetDependedTypes() => new[] { ModuleType };
}

