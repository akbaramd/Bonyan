namespace Bonyan.DependencyInjection;

/// <summary>
/// Strategy for contributing service registrations from discovered types (Strategy pattern).
/// Implementations decide which types to consider, whether they can be registered, and their lifetime/service types.
/// </summary>
public interface IServiceRegistrationContributor
{
    /// <summary>
    /// Returns candidate types from the context's assemblies to consider for registration.
    /// </summary>
    IEnumerable<Type> GetCandidateTypes(IServiceRegistrationContext context);

    /// <summary>
    /// Whether this type should be registered. Use for filtering and configuration (e.g. exclude tests, respect options).
    /// </summary>
    bool CanRegister(Type type, IServiceRegistrationContext context);

    /// <summary>
    /// Returns the registration descriptor for the type, or null to skip.
    /// Defines lifetime, replace behavior, and service types (interfaces) to register as.
    /// </summary>
    ServiceRegistrationDescriptor? GetRegistration(Type type, IServiceRegistrationContext context);
}
