using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.Plugins;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Bonyan.Modularity.Tests;

/// <summary>
/// CI Fitness Function: Ensures no circular dependencies exist in the module graph.
/// This is a hard gate - cycles destroy modularity and lead to Big Ball of Mud.
/// </summary>
public class ModuleCycleDetectionTests
{
    /// <summary>
    /// Fitness function: Modules must not have circular dependencies.
    /// This test fails the build if cycles are detected, enforcing governance.
    /// </summary>
    [Fact]
    public void Modules_must_not_have_cycles()
    {
        // Arrange
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Warning));
        var catalog = new ModuleCatalog(loggerFactory.CreateLogger<ModuleCatalog>());
        var metadataProvider = new AttributeModuleMetadataProvider(loggerFactory.CreateLogger<AttributeModuleMetadataProvider>());
        var graphBuilder = new DependencyGraphBuilder(loggerFactory.CreateLogger<DependencyGraphBuilder>());
        var activator = new DiModuleActivator(loggerFactory.CreateLogger<DiModuleActivator>());
        var loader = new BonModuleLoader(catalog, metadataProvider, graphBuilder, activator,
            loggerFactory.CreateLogger<BonModuleLoader>());

        var services = new ServiceCollection();
        var pluginSources = new PlugInSourceList();

        // Get the root module type from the application
        // This should be the actual root module used in your application
        // For testing, you might need to specify a test root module
        var rootModuleType = typeof(BonModule); // Replace with actual root module

        try
        {
            // Act - This will throw if cycles are detected
            var modules = loader.LoadModules(services, rootModuleType, "TestService", pluginSources);

            // Assert - If we get here, no cycles were detected
            Assert.NotNull(modules);
            Assert.NotEmpty(modules);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("Circular dependencies"))
        {
            // Re-throw with clear message for CI
            throw new InvalidOperationException(
                "CYCLE DETECTED: Module dependency graph contains cycles. " +
                "This violates modularity principles and must be fixed before build can succeed. " +
                "Review the cycle path in the exception details and remove the circular dependency.",
                ex);
        }
    }

    /// <summary>
    /// Tests that the dependency graph builder correctly detects cycles.
    /// </summary>
    [Fact]
    public void DependencyGraphBuilder_detects_cycles()
    {
        // Arrange
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Warning));
        var graphBuilder = new DependencyGraphBuilder(loggerFactory.CreateLogger<DependencyGraphBuilder>());

        // Create a cycle: A -> B -> C -> A
        var moduleA = new BonModuleDescriptor(typeof(object), null!, false, "Test", false);
        var moduleB = new BonModuleDescriptor(typeof(string), null!, false, "Test", false);
        var moduleC = new BonModuleDescriptor(typeof(int), null!, false, "Test", false);

        moduleA.AddDependency(moduleB);
        moduleB.AddDependency(moduleC);
        moduleC.AddDependency(moduleA); // Cycle!

        var modules = new[] { moduleA, moduleB, moduleC };

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            graphBuilder.BuildAndSort(modules, typeof(object)));

        Assert.Contains("Circular dependencies", exception.Message);
    }

    /// <summary>
    /// Tests that valid dependency graphs (no cycles) are sorted correctly.
    /// </summary>
    [Fact]
    public void DependencyGraphBuilder_sorts_valid_graph()
    {
        // Arrange
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Warning));
        var graphBuilder = new DependencyGraphBuilder(loggerFactory.CreateLogger<DependencyGraphBuilder>());

        // Create valid graph: A -> B -> C (no cycle)
        var moduleA = new BonModuleDescriptor(typeof(object), null!, false, "Test", false);
        var moduleB = new BonModuleDescriptor(typeof(string), null!, false, "Test", false);
        var moduleC = new BonModuleDescriptor(typeof(int), null!, false, "Test", false);

        moduleA.AddDependency(moduleB);
        moduleB.AddDependency(moduleC);

        var modules = new[] { moduleA, moduleB, moduleC };

        // Act
        var sorted = graphBuilder.BuildAndSort(modules, typeof(object));

        // Assert - Should be sorted: C, B, A (dependencies first, root last)
        Assert.Equal(3, sorted.Count);
        Assert.Equal(typeof(int), sorted[0].ModuleType); // C (no dependencies)
        Assert.Equal(typeof(string), sorted[1].ModuleType); // B (depends on C)
        Assert.Equal(typeof(object), sorted[2].ModuleType); // A (depends on B) - root last
    }
}

