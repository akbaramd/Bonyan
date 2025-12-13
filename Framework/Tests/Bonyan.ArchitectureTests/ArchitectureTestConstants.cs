namespace Bonyan.ArchitectureTests;

/// <summary>
/// Constants for architecture test classification and validation.
/// These define machine-checkable rules for module, plugin, and contract identification.
/// </summary>
internal static class ArchitectureTestConstants
{
    /// <summary>
    /// Company/namespace prefix for Bonyan assemblies.
    /// </summary>
    public const string CompanyPrefix = "Bonyan";

    /// <summary>
    /// Prefix identifying plugin assemblies.
    /// Plug-ins follow pattern: Bonyan.Plugin.{Domain}.{Context}.{Layer}
    /// </summary>
    public const string PluginPrefix = "Bonyan.Plugin.";

    /// <summary>
    /// Suffix identifying contracts-only assemblies.
    /// Contracts assemblies contain only interfaces and DTOs, no implementation.
    /// </summary>
    public const string ContractsSuffix = ".Contracts";

    /// <summary>
    /// Suffix identifying abstractions assemblies.
    /// </summary>
    public const string AbstractionsSuffix = ".Abstractions";

    /// <summary>
    /// Core/shared assemblies that are allowed to be referenced by any module.
    /// These represent the microkernel core and shared infrastructure.
    /// </summary>
    public static readonly HashSet<string> AllowedSharedAssemblies = new(StringComparer.OrdinalIgnoreCase)
    {
        "Bonyan",
        "Bonyan.Modularity",
        "Bonyan.Modularity.Abstractions",
        "Bonyan.Plugins",
        "Bonyan.Reflection",
        "Bonyan.DependencyInjection",
        "Bonyan.Exceptions",
        "Bonyan.Core",
        "Bonyan.SharedKernel"
    };

    /// <summary>
    /// Core persistence assemblies that plugins must NOT reference.
    /// Plug-ins should not directly access the core/shared database.
    /// Core owns persistence responsibility; plugins receive data via contracts.
    /// </summary>
    public static readonly HashSet<string> ForbiddenCorePersistenceAssemblies = new(StringComparer.OrdinalIgnoreCase)
    {
        "Bonyan.EntityFrameworkCore",
        "Bonyan.EntityFrameworkCore.SqlServer",
        "Bonyan.Core.EntityFrameworkCore",
        "Bonyan.Layer.Domain",
        "Bonyan.Layer.Domain.EntityFrameworkCore"
    };

    /// <summary>
    /// Module layer suffixes for classification.
    /// Modules follow pattern: Bonyan.{Module}.{Layer}
    /// </summary>
    public static readonly HashSet<string> ModuleLayerSuffixes = new(StringComparer.OrdinalIgnoreCase)
    {
        "Contracts",
        "Domain",
        "Application",
        "Infrastructure",
        "Host",
        "WebApi",
        "EntityFrameworkCore"
    };
}

