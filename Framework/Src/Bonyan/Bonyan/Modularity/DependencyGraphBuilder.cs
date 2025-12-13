using System.Collections.Generic;
using System.Linq;
using Bonyan.Exceptions;
using Bonyan.Modularity.Abstractions;
using Microsoft.Extensions.Logging;

namespace Bonyan.Modularity;

/// <summary>
/// Builds dependency graphs, performs topological sort, and detects cycles.
/// Part of the microkernel core architecture - enforces governance to prevent Big Ball of Mud.
/// </summary>
public sealed class DependencyGraphBuilder : IDependencyGraphBuilder
{
    private readonly ILogger<DependencyGraphBuilder>? _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="DependencyGraphBuilder"/>.
    /// </summary>
    /// <param name="logger">Optional logger for diagnostic information.</param>
    public DependencyGraphBuilder(ILogger<DependencyGraphBuilder>? logger = null)
    {
        _logger = logger;
    }

    /// <summary>
    /// Builds a dependency graph from module descriptors and performs topological sort.
    /// </summary>
    /// <param name="modules">Module descriptors to build graph from.</param>
    /// <param name="rootModuleType">The root module type (will be placed last).</param>
    /// <returns>Topologically sorted list of module descriptors.</returns>
    /// <exception cref="InvalidOperationException">Thrown if circular dependencies are detected.</exception>
    public IReadOnlyList<BonModuleDescriptor> BuildAndSort(
        IReadOnlyList<BonModuleDescriptor> modules,
        Type rootModuleType)
    {
        if (modules == null || modules.Count == 0)
        {
            _logger?.LogWarning("No modules provided for graph building");
            return Array.Empty<BonModuleDescriptor>();
        }

        // Build adjacency list and in-degree map
        var moduleMap = modules.ToDictionary(m => m.ModuleType, m => m);
        var adjacencyList = new Dictionary<Type, List<Type>>();
        var inDegree = new Dictionary<Type, int>();

        // Initialize
        foreach (var module in modules)
        {
            adjacencyList[module.ModuleType] = new List<Type>();
            inDegree[module.ModuleType] = 0;
        }

        // Build edges
        foreach (var module in modules)
        {
            foreach (var dependency in module.Dependencies)
            {
                if (!adjacencyList.ContainsKey(dependency.ModuleType))
                {
                    throw new BonException(
                        $"Module {module.ModuleType.FullName} depends on {dependency.ModuleType.FullName}, " +
                        $"but {dependency.ModuleType.FullName} is not in the module list.");
                }

                adjacencyList[dependency.ModuleType].Add(module.ModuleType);
                inDegree[module.ModuleType]++;
            }
        }

        // Detect cycles before sorting
        var cycles = DetectCycles(modules);
        if (cycles.Any())
        {
            var cycleMessages = cycles.Select(cycle =>
                string.Join(" → ", cycle.Select(t => t.Name)) + " → " + cycle.First().Name);

            var message = $"Circular dependencies detected:\n{string.Join("\n", cycleMessages)}";
            _logger?.LogError(message);
            throw new InvalidOperationException(message);
        }

        // Kahn's algorithm for topological sort
        var queue = new Queue<Type>();
        foreach (var kvp in inDegree)
        {
            if (kvp.Value == 0)
            {
                queue.Enqueue(kvp.Key);
            }
        }

        var sorted = new List<BonModuleDescriptor>();
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            var module = moduleMap[current];
            sorted.Add(module);

            foreach (var neighbor in adjacencyList[current])
            {
                inDegree[neighbor]--;
                if (inDegree[neighbor] == 0)
                {
                    queue.Enqueue(neighbor);
                }
            }
        }

        // Move root module to end
        var rootModule = sorted.FirstOrDefault(m => m.ModuleType == rootModuleType);
        if (rootModule != null)
        {
            sorted.Remove(rootModule);
            sorted.Add(rootModule);
        }

        _logger?.LogDebug("Topological sort completed: {ModuleCount} modules in load order", sorted.Count);

        return sorted;
    }

    /// <summary>
    /// Detects cycles in the dependency graph using DFS.
    /// </summary>
    /// <param name="modules">Module descriptors to check.</param>
    /// <returns>List of cycle paths if any cycles are found, empty list otherwise.</returns>
    public IReadOnlyList<IReadOnlyList<Type>> DetectCycles(IReadOnlyList<BonModuleDescriptor> modules)
    {
        if (modules == null || modules.Count == 0)
            return Array.Empty<IReadOnlyList<Type>>();

        var moduleMap = modules.ToDictionary(m => m.ModuleType, m => m);
        var visited = new HashSet<Type>();
        var recStack = new HashSet<Type>();
        var cycles = new List<List<Type>>();
        var path = new List<Type>();

        foreach (var module in modules)
        {
            if (!visited.Contains(module.ModuleType))
            {
                DetectCyclesDfs(module.ModuleType, moduleMap, visited, recStack, path, cycles);
            }
        }

        return cycles.Select(c => (IReadOnlyList<Type>)c.ToList()).ToList();
    }

    private void DetectCyclesDfs(
        Type current,
        Dictionary<Type, BonModuleDescriptor> moduleMap,
        HashSet<Type> visited,
        HashSet<Type> recStack,
        List<Type> path,
        List<List<Type>> cycles)
    {
        visited.Add(current);
        recStack.Add(current);
        path.Add(current);

        if (moduleMap.TryGetValue(current, out var module))
        {
            foreach (var dependency in module.Dependencies)
            {
                var depType = dependency.ModuleType;
                if (!visited.Contains(depType))
                {
                    DetectCyclesDfs(depType, moduleMap, visited, recStack, path, cycles);
                }
                else if (recStack.Contains(depType))
                {
                    // Cycle detected - extract the cycle path
                    var cycleStart = path.IndexOf(depType);
                    var cycle = path.Skip(cycleStart).ToList();
                    cycle.Add(depType); // Close the cycle
                    cycles.Add(cycle);
                }
            }
        }

        recStack.Remove(current);
        path.RemoveAt(path.Count - 1);
    }
}

