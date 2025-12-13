using System.Reflection;
using Bonyan.Modularity.Abstractions;
using Xunit;

namespace Bonyan.ArchitectureTests;

/// <summary>
/// CI Fitness Function: Ensures no circular dependencies exist in the module graph.
/// This is a hard gate - cycles destroy modularity and lead to Big Ball of Mud.
/// </summary>
public sealed class ModuleCycleDetectionTests
{
    /// <summary>
    /// Fitness function: Modules must not have circular dependencies.
    /// This test fails the build if cycles are detected, enforcing governance.
    /// </summary>
    [Fact]
    public void Modules_must_not_have_cycles()
    {
        // Arrange
        var asms = AssemblyDiscovery.LoadBonyanAssemblies();
        var moduleTypes = ModuleDiscovery.FindModuleTypes(asms, typeof(IBonModule));

        if (moduleTypes.Count == 0)
        {
            // No modules found - this might be expected in some test scenarios
            return;
        }

        // Build dependency graph
        var edges = moduleTypes.ToDictionary(
            m => m,
            m => (IReadOnlyList<Type>)ModuleDiscovery.GetDependencies(m));

        // Act - Detect cycles
        var cycle = CycleDetector.FindCycle(edges);

        // Assert - Fail if cycle found
        if (cycle != null)
        {
            var path = CycleDetector.FormatCycle(cycle);
            throw new InvalidOperationException(
                $"CYCLE DETECTED: Module dependency graph contains cycles.\n\n" +
                $"Cycle path: {path}\n\n" +
                $"This violates modularity principles and must be fixed before build can succeed.\n" +
                $"Fix options:\n" +
                $"  1. Extract shared functionality to a common module\n" +
                $"  2. Invert the dependency (use dependency inversion principle)\n" +
                $"  3. Introduce an adapter/mediator pattern\n" +
                $"  4. Refactor to remove the circular dependency");
        }
    }
}

