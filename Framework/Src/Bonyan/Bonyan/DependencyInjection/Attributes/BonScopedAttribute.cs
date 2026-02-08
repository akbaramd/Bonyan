namespace Bonyan.DependencyInjection;

/// <summary>
/// Registers the class as scoped. Uses TryAdd (does not replace existing registration).
/// For replace behavior or multiple service types, use <see cref="BonServiceAttribute"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class BonScopedAttribute : BonServiceAttribute
{
    public BonScopedAttribute(bool replaceExisting = false, params Type[]? serviceTypes)
        : base(Microsoft.Extensions.DependencyInjection.ServiceLifetime.Scoped, replaceExisting, serviceTypes)
    {
    }
}
