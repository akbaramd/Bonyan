using System.Text;
using Bonyan.Modularity.Abstractions;
using Microsoft.Extensions.Logging;

namespace Bonyan.Modularity;

/// <summary>
/// Visualizes module dependency graphs in various formats for logging and debugging.
/// </summary>
public static class ModuleGraphVisualizer
{
    /// <summary>
    /// Generates a tree-style visualization of the module dependency graph starting from the root module.
    /// Shows forward dependencies (what each module depends on) as children in the tree.
    /// The root module is the entry point specified in Program.cs.
    /// </summary>
    public static string GenerateTreeVisualization(IReadOnlyList<BonModuleDescriptor> modules, Type rootModuleType)
    {
        if (modules == null || modules.Count == 0)
            return "No modules to visualize.";

        var sb = new StringBuilder();
        // Headers removed - will be handled by border box in BonModuleLoader

        var moduleMap = modules.ToDictionary(m => m.ModuleType, m => m);

        // Always use the root module as the entry point (the one specified in Program.cs)
        if (rootModuleType == null || !moduleMap.ContainsKey(rootModuleType))
        {
            // Fallback: find modules with no dependencies if root not found
            var entryPoints = modules
                .Where(m => m.Dependencies.Count == 0)
                .Select(m => m.ModuleType)
                .ToList();
            
            if (entryPoints.Count == 0)
            {
                return "No root module or entry points found.";
            }

            // Use first entry point as root
            rootModuleType = entryPoints[0];
        }

        // Build dependency tree starting from root module
        var visited = new HashSet<Type>();
        BuildDependencyTreeFromRoot(sb, rootModuleType, moduleMap, visited, "", true, true);

        // Handle any unvisited modules (shouldn't happen in a well-formed graph, but handle gracefully)
        var unvisited = modules.Where(m => !visited.Contains(m.ModuleType)).ToList();
        if (unvisited.Any())
        {
            sb.AppendLine();
            sb.AppendLine("Additional modules (not reachable from root):");
            foreach (var module in unvisited)
            {
                if (!visited.Contains(module.ModuleType))
                {
                    BuildDependencyTreeFromRoot(sb, module.ModuleType, moduleMap, visited, "", true, true);
                }
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Builds a dependency tree showing forward dependencies from the root module.
    /// Shows what the root module depends on, and recursively shows what those dependencies depend on.
    /// </summary>
    private static void BuildDependencyTreeFromRoot(
        StringBuilder sb,
        Type moduleType,
        Dictionary<Type, BonModuleDescriptor> moduleMap,
        HashSet<Type> visited,
        string prefix,
        bool isLast,
        bool isRoot)
    {
        // Skip if already visited (prevents duplicates and cycles)
        if (visited.Contains(moduleType))
        {
            // Show reference but don't recurse
            var refConnector = isRoot ? "" : (isLast ? "└── " : "├── ");
            sb.AppendLine($"{prefix}{refConnector}{moduleType.Name} [already shown above]");
            return;
        }

        visited.Add(moduleType);
        var module = moduleMap[moduleType];

        // Print current module
        var connector = isRoot ? "" : (isLast ? "└── " : "├── ");
        var moduleInfo = $"{connector}{moduleType.Name}";
        
        if (module.IsPluginModule)
        {
            moduleInfo += " [Plugin]";
        }

        if (isRoot)
        {
            moduleInfo += " [Root/Entry Point]";
        }

        sb.AppendLine(prefix + moduleInfo);

        // Show dependencies as children (forward dependency tree)
        if (module.Dependencies.Any())
        {
            var newPrefix = isRoot ? "" : (isLast ? "    " : "│   ");
            var dependencies = module.Dependencies.Select(d => d.ModuleType).ToList();
            
            for (int i = 0; i < dependencies.Count; i++)
            {
                var dependency = dependencies[i];
                var dependencyIsLast = i == dependencies.Count - 1;
                BuildDependencyTreeFromRoot(sb, dependency, moduleMap, visited, prefix + newPrefix, dependencyIsLast, false);
            }
        }
    }

    /// <summary>
    /// Generates a load order visualization showing modules in their initialization sequence.
    /// Shows dependencies for each module when it loads.
    /// </summary>
    public static string GenerateLoadOrderVisualization(IReadOnlyList<BonModuleDescriptor> sortedModules)
    {
        if (sortedModules == null || sortedModules.Count == 0)
            return "No modules in load order.";

        var sb = new StringBuilder();
        // Headers removed - will be handled by border box in BonModuleLoader

        var visited = new HashSet<Type>();

        for (int i = 0; i < sortedModules.Count; i++)
        {
            var module = sortedModules[i];
            
            // Skip if already shown (no duplicates)
            if (visited.Contains(module.ModuleType))
                continue;

            visited.Add(module.ModuleType);
            var step = $"{i + 1:D2}";
            var moduleName = module.ModuleType.Name;
            var pluginTag = module.IsPluginModule ? " [Plugin]" : "";
            
            sb.Append($"{step}. {moduleName}{pluginTag}");

            // Show dependencies when module loads
            if (module.Dependencies.Any())
            {
                var deps = string.Join(", ", module.Dependencies.Select(d => d.ModuleType.Name));
                sb.AppendLine($" → depends on: {deps}");
            }
            else
            {
                sb.AppendLine(" [Entry Point - no dependencies]");
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Generates a DOT format graph for Graphviz visualization.
    /// </summary>
    public static string GenerateDotGraph(IReadOnlyList<BonModuleDescriptor> modules)
    {
        if (modules == null || modules.Count == 0)
            return "digraph { }";

        var sb = new StringBuilder();
        sb.AppendLine("digraph ModuleDependencies {");
        sb.AppendLine("  rankdir=TB;");
        sb.AppendLine("  node [shape=box, style=rounded];");
        sb.AppendLine();

        // Add nodes
        foreach (var module in modules)
        {
            var nodeName = SanitizeNodeName(module.ModuleType.Name);
            var label = module.ModuleType.Name;
            if (module.IsPluginModule)
            {
                label += "\\n[Plugin]";
            }
            sb.AppendLine($"  {nodeName} [label=\"{label}\"];");
        }

        sb.AppendLine();

        // Add edges (dependencies)
        foreach (var module in modules)
        {
            var fromNode = SanitizeNodeName(module.ModuleType.Name);
            foreach (var dep in module.Dependencies)
            {
                var toNode = SanitizeNodeName(dep.ModuleType.Name);
                sb.AppendLine($"  {toNode} -> {fromNode};");
            }
        }

        sb.AppendLine("}");
        return sb.ToString();
    }

    private static string SanitizeNodeName(string name)
    {
        return name.Replace(".", "_").Replace("-", "_").Replace(" ", "_");
    }

    /// <summary>
    /// Generates a detailed dependency matrix showing all relationships.
    /// </summary>
    public static string GenerateDependencyMatrix(IReadOnlyList<BonModuleDescriptor> modules)
    {
        if (modules == null || modules.Count == 0)
            return "No modules for dependency matrix.";

        var sb = new StringBuilder();
        sb.AppendLine("Module Dependency Matrix:");
        sb.AppendLine(new string('=', 80));

        var moduleList = modules.ToList();
        var maxNameLength = moduleList.Max(m => m.ModuleType.Name.Length);
        var nameWidth = Math.Max(maxNameLength, 20);

        // Header
        sb.Append("".PadRight(nameWidth + 2));
        foreach (var module in moduleList)
        {
            sb.Append(module.ModuleType.Name.Substring(0, Math.Min(8, module.ModuleType.Name.Length)).PadRight(10));
        }
        sb.AppendLine();

        // Rows
        foreach (var module in moduleList)
        {
            sb.Append(module.ModuleType.Name.PadRight(nameWidth + 2));
            foreach (var other in moduleList)
            {
                var hasDep = module.Dependencies.Any(d => d.ModuleType == other.ModuleType);
                sb.Append((hasDep ? "X" : ".").PadRight(10));
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }
}

