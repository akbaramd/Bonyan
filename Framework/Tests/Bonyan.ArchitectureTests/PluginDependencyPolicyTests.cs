using Bonyan.Modularity.Abstractions;
using Xunit;
using Xunit.Sdk;

namespace Bonyan.ArchitectureTests;

/// <summary>
/// Enforces Rule 4: No plug-in → plug-in dependencies by default.
/// Plugin-to-plugin dependencies require explicit Reason to enforce intentional coupling.
/// </summary>
public sealed class PluginDependencyPolicyTests
{
    /// <summary>
    /// Fitness function: Plugin-to-plugin dependencies require Reason.
    /// </summary>
    [Fact]
    public void Plugin_to_plugin_dependencies_require_reason()
    {
        // Arrange
        var asms = AssemblyDiscovery.LoadBonyanAssemblies();
        var pluginAssemblies = asms.Where(a => ModuleDiscovery.IsPluginAssembly(a)).ToList();

        if (pluginAssemblies.Count == 0)
        {
            // No plugins found - this is acceptable
            return;
        }

        var moduleTypes = ModuleDiscovery.FindModuleTypes(pluginAssemblies, typeof(IBonModule));
        var violations = new List<string>();

        foreach (var moduleType in moduleTypes)
        {
            var depends = ModuleDiscovery.GetDependencyAttributes(moduleType);
            
            foreach (var dep in depends)
            {
                var depAsm = dep.ModuleType.Assembly;
                
                // Only check plugin->plugin dependencies
                if (!ModuleDiscovery.IsPluginAssembly(depAsm))
                    continue;

                // Check if Reason is provided
                if (string.IsNullOrWhiteSpace(dep.Reason))
                {
                    violations.Add(
                        $"{moduleType.FullName} depends on plugin {dep.ModuleType.FullName} but has no Reason.\n" +
                        $"Plugin->plugin dependencies are forbidden by default.\n" +
                        $"Fix: Add Reason property to [DependsOn] attribute or refactor to remove dependency.");
                }
            }
        }

        // Assert - Fail if violations found
        if (violations.Any())
        {
            var message = "PLUGIN-TO-PLUGIN DEPENDENCY VIOLATIONS:\n\n" +
                         string.Join("\n\n", violations) +
                         "\n\nPlugin-to-plugin dependencies require explicit Reason to enforce intentional coupling.\n" +
                         "Example: [DependsOn(typeof(OtherPlugin), Reason = \"Shared payment processing\")]";
            
            throw new XunitException(message);
        }
    }
}

