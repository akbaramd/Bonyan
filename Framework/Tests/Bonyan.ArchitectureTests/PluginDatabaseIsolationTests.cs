using Xunit;
using Xunit.Sdk;

namespace Bonyan.ArchitectureTests;

/// <summary>
/// Enforces Rule 12: Plug-ins must not touch the centrally shared DB.
/// Core owns persistence responsibility; plugins receive data via contracts.
/// </summary>
public sealed class PluginDatabaseIsolationTests
{
    /// <summary>
    /// Fitness function: Plugins must not reference core persistence assemblies.
    /// </summary>
    [Fact]
    public void Plugins_must_not_reference_core_persistence_assemblies()
    {
        // Arrange
        var asms = AssemblyDiscovery.LoadBonyanAssemblies();
        var violations = new List<string>();

        foreach (var asm in asms.Where(ModuleDiscovery.IsPluginAssembly))
        {
            var asmName = asm.GetName().Name ?? "";
            var refs = asm.GetReferencedAssemblies()
                .Select(r => r.Name ?? "")
                .Where(n => !string.IsNullOrEmpty(n))
                .ToList();

            // Check for forbidden core persistence assemblies
            var forbidden = refs.FirstOrDefault(r => 
                ArchitectureTestConstants.ForbiddenCorePersistenceAssemblies.Contains(r));
            
            if (forbidden != null)
            {
                violations.Add(
                    $"{asmName} references {forbidden}.\n" +
                    $"Plugins must not use the core/shared database.\n" +
                    $"Fix: Receive data via contracts/interfaces, not direct DB access.");
            }
        }

        // Assert - Fail if violations found
        if (violations.Any())
        {
            var message = "PLUGIN DATABASE ISOLATION VIOLATIONS:\n\n" +
                         string.Join("\n\n", violations) +
                         "\n\nPlugins must not directly access core persistence.\n" +
                         "Core owns persistence responsibility; plugins receive data via contracts.";
            
            throw new XunitException(message);
        }
    }
}

