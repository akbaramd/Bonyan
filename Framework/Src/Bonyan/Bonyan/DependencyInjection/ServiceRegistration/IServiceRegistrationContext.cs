using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.DependencyInjection;

/// <summary>
/// Context passed to service registration contributors (DIP).
/// Provides assemblies to scan, the service collection to register into, and optional configuration.
/// </summary>
public interface IServiceRegistrationContext
{
    /// <summary>Assemblies to consider for type discovery.</summary>
    IReadOnlyList<Assembly> Assemblies { get; }

    /// <summary>Service collection to register descriptors into.</summary>
    IServiceCollection Services { get; }

    /// <summary>Optional: get custom data stored by key (for configuration/filtering).</summary>
    object? GetData(string key);

    /// <summary>Optional: set custom data (for configuration/filtering).</summary>
    void SetData(string key, object? value);
}
