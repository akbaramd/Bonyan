namespace Bonyan.ArchitectureTests;

/// <summary>
/// Detects cycles in dependency graphs using DFS.
/// Returns actionable cycle paths for debugging.
/// </summary>
internal static class CycleDetector
{
    /// <summary>
    /// Finds a cycle in the dependency graph if one exists.
    /// </summary>
    /// <param name="edges">Dictionary mapping each node to its dependencies.</param>
    /// <returns>Cycle path if found (A -> B -> C -> A), null otherwise.</returns>
    public static IReadOnlyList<Type>? FindCycle(Dictionary<Type, IReadOnlyList<Type>> edges)
    {
        var state = new Dictionary<Type, int>(); // 0=unvisited, 1=visiting, 2=done
        var stack = new Stack<Type>();
        var indexInStack = new Dictionary<Type, int>();

        foreach (var node in edges.Keys)
        {
            if (!state.TryGetValue(node, out var s)) s = 0;
            if (s == 0)
            {
                var cycle = Dfs(node);
                if (cycle != null) return cycle;
            }
        }
        return null;

        IReadOnlyList<Type>? Dfs(Type node)
        {
            state[node] = 1;
            indexInStack[node] = stack.Count;
            stack.Push(node);

            if (edges.TryGetValue(node, out var deps))
            {
                foreach (var dep in deps)
                {
                    // Ignore missing nodes (optional deps) - they should be added to graph upstream
                    if (!edges.ContainsKey(dep)) continue;

                    if (!state.TryGetValue(dep, out var s)) s = 0;

                    if (s == 0)
                    {
                        var cycle = Dfs(dep);
                        if (cycle != null) return cycle;
                    }
                    else if (s == 1)
                    {
                        // Back edge found: extract cycle
                        var startIndex = indexInStack[dep];
                        var arr = stack.Reverse().ToList(); // bottom->top
                        var cycleNodes = arr.Skip(startIndex).ToList();
                        cycleNodes.Add(dep); // Close the cycle
                        return cycleNodes;
                    }
                }
            }

            stack.Pop();
            indexInStack.Remove(node);
            state[node] = 2;
            return null;
        }
    }

    /// <summary>
    /// Formats a cycle path as a human-readable string.
    /// </summary>
    /// <param name="cycle">The cycle path.</param>
    /// <returns>Formatted cycle string (e.g., "ModuleA -> ModuleB -> ModuleC -> ModuleA").</returns>
    public static string FormatCycle(IReadOnlyList<Type> cycle)
    {
        return string.Join(" -> ", cycle.Select(t => t.Name)) + " -> " + cycle.First().Name;
    }
}

