namespace Bonyan.DependencyInjection;

/// <summary>
/// When applied to a type, conventional registrars will skip it (no automatic registration).
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = true, AllowMultiple = false)]
public class DisableConventionalRegistrationAttribute : Attribute
{
}
