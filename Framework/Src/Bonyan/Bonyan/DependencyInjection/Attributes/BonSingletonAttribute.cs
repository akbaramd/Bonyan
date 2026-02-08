namespace Bonyan.DependencyInjection;

/// <summary>
/// Registers the class as a singleton. Uses TryAdd (does not replace existing registration).
/// For replace behavior or multiple service types, use <see cref="BonServiceAttribute"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class BonSingletonAttribute : BonServiceAttribute
{
    public BonSingletonAttribute(bool replaceExisting = false, params Type[]? serviceTypes)
        : base(Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton, replaceExisting, serviceTypes)
    {
    }
}
