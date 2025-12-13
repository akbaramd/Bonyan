using System.Reflection;
using Xunit;
using Xunit.Sdk;

namespace Bonyan.ArchitectureTests;

/// <summary>
/// Enforces Rule 3: Weak coupling across module boundaries.
/// Cross-module references must target *.Contracts or *.Abstractions only.
/// This minimizes connascence crossing boundaries (connascence locality principle).
/// </summary>
public sealed class CrossModuleReferenceTests
{
    /// <summary>
    /// Fitness function: Cross-module references must be contracts-only.
    /// </summary>
    [Fact]
    public void Cross_module_references_must_be_contracts_only()
    {
        // Arrange
        var asms = AssemblyDiscovery.LoadBonyanAssemblies();
        var violations = new List<string>();

        foreach (var asm in asms)
        {
            var asmName = asm.GetName().Name ?? "";
            
            // Only check Bonyan assemblies
            if (!asmName.StartsWith(ArchitectureTestConstants.CompanyPrefix + ".", StringComparison.OrdinalIgnoreCase))
                continue;

            // Skip core/shared assemblies - they can reference each other
            if (ArchitectureTestConstants.AllowedSharedAssemblies.Contains(asmName))
                continue;

            // Skip contracts/abstractions assemblies - they're meant to be referenced
            if (ModuleDiscovery.IsContractsAssembly(asm))
                continue;

            // Get referenced assemblies
            var referenced = asm.GetReferencedAssemblies()
                .Select(r => r.Name ?? "")
                .Where(n => n.StartsWith(ArchitectureTestConstants.CompanyPrefix + ".", StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var refName in referenced)
            {
                // Allow referencing core/shared assemblies
                if (ArchitectureTestConstants.AllowedSharedAssemblies.Contains(refName))
                    continue;

                // Allow referencing any *.Contracts or *.Abstractions
                if (refName.EndsWith(ArchitectureTestConstants.ContractsSuffix, StringComparison.OrdinalIgnoreCase) ||
                    refName.EndsWith(ArchitectureTestConstants.AbstractionsSuffix, StringComparison.OrdinalIgnoreCase))
                    continue;

                // Everything else is a violation (tight cross-module coupling)
                violations.Add($"{asmName} references {refName}. Cross-module references must target *.Contracts or *.Abstractions only.");
            }
        }

        // Assert - Fail if violations found
        if (violations.Any())
        {
            var message = "CROSS-MODULE REFERENCE VIOLATIONS:\n\n" +
                         string.Join("\n", violations) +
                         "\n\nFix: Reference only *.Contracts or *.Abstractions assemblies from other modules.\n" +
                         "This enforces weak coupling across module boundaries (connascence locality).";
            
            throw new XunitException(message);
        }
    }
}

