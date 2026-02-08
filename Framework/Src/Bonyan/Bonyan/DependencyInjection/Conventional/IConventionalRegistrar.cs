using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.DependencyInjection;

/// <summary>
/// ABP-style conventional registrar: discovers candidate types and provides CanRegister + GetRegistration.
/// Adding to <see cref="IServiceCollection"/> is handled by <see cref="ServiceCollectionConventionalRegistrarExtensions.AddAssembly"/> (Single Responsibility).
/// </summary>
public interface IConventionalRegistrar
{
    /// <summary>Returns candidate types from the assembly to consider (e.g. filter by attribute or convention).</summary>
    IEnumerable<Type> GetCandidateTypes(Assembly assembly);

    /// <summary>Whether this type should be registered. Override for filtering (e.g. exclude disabled, validate).</summary>
    bool CanRegister(Type type);

    /// <summary>Returns the registration descriptor for the type, or null to skip. Defines lifetime, replace behavior, and service types.</summary>
    ServiceRegistrationDescriptor? GetRegistration(Type type);
}
