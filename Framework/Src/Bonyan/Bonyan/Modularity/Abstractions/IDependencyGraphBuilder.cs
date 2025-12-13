namespace Bonyan.Modularity.Abstractions;

/// <summary>
/// Builds dependency graph, performs topological sort, and detects cycles.
/// Part of the microkernel core architecture.
/// </summary>
public interface IDependencyGraphBuilder
{
    /// <summary>
    /// Builds a dependency graph from module descriptors and performs topological sort.
    /// </summary>
    /// <param name="modules">Module descriptors to build graph from.</param>
    /// <param name="rootModuleType">The root module type (will be placed last).</param>
    /// <returns>Topologically sorted list of module descriptors.</returns>
    /// <exception cref="InvalidOperationException">Thrown if circular dependencies are detected.</exception>
    IReadOnlyList<BonModuleDescriptor> BuildAndSort(IReadOnlyList<BonModuleDescriptor> modules, Type rootModuleType);

    /// <summary>
    /// Detects cycles in the dependency graph.
    /// </summary>
    /// <param name="modules">Module descriptors to check.</param>
    /// <returns>List of cycle paths if any cycles are found, empty list otherwise.</returns>
    IReadOnlyList<IReadOnlyList<Type>> DetectCycles(IReadOnlyList<BonModuleDescriptor> modules);
}

