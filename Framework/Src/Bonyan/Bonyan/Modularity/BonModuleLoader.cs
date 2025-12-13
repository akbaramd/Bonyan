using System.Linq;
using Bonyan.Core;
using Bonyan.Exceptions;
using Bonyan.Modularity.Abstractions;
using Bonyan.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Bonyan.Modularity;

/// <summary>
/// Orchestrates module loading using the microkernel architecture components.
/// Keeps the core small and delegates to specialized components (catalog, metadata, graph, activator).
/// </summary>
public sealed class BonModuleLoader : IBonModuleLoader
{
    private readonly IModuleCatalog _catalog;
    private readonly IModuleMetadataProvider _metadataProvider;
    private readonly IDependencyGraphBuilder _graphBuilder;
    private readonly IModuleActivator _activator;
    private readonly ILogger<BonModuleLoader>? _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="BonModuleLoader"/>.
    /// </summary>
    /// <param name="catalog">Module catalog for discovery.</param>
    /// <param name="metadataProvider">Metadata provider for reading dependencies.</param>
    /// <param name="graphBuilder">Graph builder for dependency resolution and cycle detection.</param>
    /// <param name="activator">Module activator for creating instances.</param>
    /// <param name="logger">Optional logger for diagnostic information.</param>
    public BonModuleLoader(
        IModuleCatalog catalog,
        IModuleMetadataProvider metadataProvider,
        IDependencyGraphBuilder graphBuilder,
        IModuleActivator activator,
        ILogger<BonModuleLoader>? logger = null)
    {
        _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
        _metadataProvider = metadataProvider ?? throw new ArgumentNullException(nameof(metadataProvider));
        _graphBuilder = graphBuilder ?? throw new ArgumentNullException(nameof(graphBuilder));
        _activator = activator ?? throw new ArgumentNullException(nameof(activator));
        _logger = logger;
    }

    /// <summary>
    /// Loads modules using the microkernel pipeline: catalog → metadata → graph → activation.
    /// </summary>
    public BonModuleDescriptor[] LoadModules(
        IServiceCollection services,
        Type startupModuleType,
        string serviceKey,
        string serviceTitle,
        PlugInSourceList plugInSources)
    {
        Check.NotNull(services, nameof(services));
        Check.NotNull(startupModuleType, nameof(startupModuleType));
        Check.NotNull(plugInSources, nameof(plugInSources));
        
        // Validate service key
        if (string.IsNullOrWhiteSpace(serviceKey))
        {
            throw new ArgumentException("Service key is required and cannot be null, empty, or whitespace.", nameof(serviceKey));
        }
        
        // Validate service title
        if (string.IsNullOrWhiteSpace(serviceTitle))
        {
            throw new ArgumentException("Service title is required and cannot be null, empty, or whitespace.", nameof(serviceTitle));
        }

        // Step 1: Start from root entry point and discover all modules recursively
        // Create instances and find dependencies by reading DependedModules from each instance
        var allDiscoveredTypes = new HashSet<Type> { startupModuleType };
        var createdInstances = new Dictionary<Type, IBonModule>();
        var visitedTypes = new HashSet<Type>();
        
        // Recursively discover and create instances starting from root
        DiscoverAndCreateModulesRecursively(startupModuleType, services, allDiscoveredTypes, createdInstances, visitedTypes);
        
        // Also discover plugin modules from plugin sources
        var pluginModuleTypes = _catalog.DiscoverModuleTypes(startupModuleType, plugInSources)
            .Where(t => !allDiscoveredTypes.Contains(t));
        
        foreach (var pluginType in pluginModuleTypes)
        {
            if (!allDiscoveredTypes.Contains(pluginType))
            {
                allDiscoveredTypes.Add(pluginType);
                DiscoverAndCreateModulesRecursively(pluginType, services, allDiscoveredTypes, createdInstances, visitedTypes);
            }
        }

        // Step 3: Create descriptors for all discovered modules
        var coreModuleTypes = BonyanModuleHelper.FindAllModuleTypes(startupModuleType).ToHashSet();
        var descriptors = new List<BonModuleDescriptor>();
        var descriptorMap = new Dictionary<Type, BonModuleDescriptor>();

        foreach (var moduleType in allDiscoveredTypes)
        {
            var isPlugin = !coreModuleTypes.Contains(moduleType);

            // Use already created instance if available, otherwise create new one
            var instance = createdInstances.ContainsKey(moduleType)
                ? createdInstances[moduleType]
                : _activator.CreateAndRegister(services, moduleType);
                
            var descriptor = new BonModuleDescriptor(
                moduleType,
                instance,
                isLoaded: true,
                serviceKey,
                isPlugin);

            descriptors.Add(descriptor);
            descriptorMap[moduleType] = descriptor;
        }

        // Step 4: Build dependency graph from constructor-based dependencies only
        foreach (var moduleType in allDiscoveredTypes)
        {
            var descriptor = descriptorMap[moduleType];
            var instance = descriptor.Instance;
            
            // Read dependencies from constructor-based DependOn() calls (DependedModules property)
            if (instance != null && instance.DependedModules != null && instance.DependedModules.Count > 0)
            {
                foreach (var depType in instance.DependedModules)
                {
                    if (depType != null)
                    {
                        if (!descriptorMap.TryGetValue(depType, out var depDescriptor))
                        {
                            _logger?.LogError(
                                "Dependency {DependencyType} for module {ModuleType} not found in discovered modules. " +
                                "This indicates a missing module or discovery issue.",
                                depType.FullName, moduleType.FullName);
                            throw new BonException(
                                $"Could not find depended module {depType.AssemblyQualifiedName} " +
                                $"for {moduleType.AssemblyQualifiedName}.");
                        }

                        descriptor.AddDependency(depDescriptor);
                    }
                }
            }
        }

        // Step 5: Build graph and sort (includes cycle detection)
        var sortedDescriptors = _graphBuilder.BuildAndSort(descriptors, startupModuleType);

        // Generate comprehensive project info and graph (shown once after all modules loaded)
        LogProjectInfoAndGraph(serviceKey, serviceTitle, startupModuleType, sortedDescriptors);

        return sortedDescriptors.ToArray();
    }

    /// <summary>
    /// Recursively discovers and creates module instances starting from a root module.
    /// Creates instance, reads dependencies from DependedModules, and recursively processes each dependency.
    /// This ensures all modules are discovered and created BEFORE configuration phase.
    /// </summary>
    private void DiscoverAndCreateModulesRecursively(
        Type moduleType,
        IServiceCollection services,
        HashSet<Type> allDiscoveredTypes,
        Dictionary<Type, IBonModule> createdInstances,
        HashSet<Type> visitedTypes)
    {
        // Prevent infinite recursion (circular dependencies)
        if (visitedTypes.Contains(moduleType))
        {
            return;
        }
        
        visitedTypes.Add(moduleType);
        
        // Create instance if not already created
        if (!createdInstances.ContainsKey(moduleType))
        {
            try
            {
                var instance = _activator.CreateAndRegister(services, moduleType);
                createdInstances[moduleType] = instance;
                
                _logger?.LogDebug("Created instance of module {ModuleType}", moduleType.FullName);
                
                // Read dependencies from constructor (DependedModules property)
                if (instance != null && instance.DependedModules != null && instance.DependedModules.Count > 0)
                {
                    foreach (var depType in instance.DependedModules)
                    {
                        if (depType != null)
                        {
                            // Add to discovered types
                            if (!allDiscoveredTypes.Contains(depType))
                            {
                                allDiscoveredTypes.Add(depType);
                            }
                            
                            // Recursively discover and create dependency modules
                            DiscoverAndCreateModulesRecursively(depType, services, allDiscoveredTypes, createdInstances, visitedTypes);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, 
                    "Failed to create instance of module {ModuleType}. " +
                    "This may indicate missing dependencies or configuration issues.",
                    moduleType.FullName);
                throw;
            }
        }
        
        visitedTypes.Remove(moduleType); // Allow revisiting in different branches
    }

    /// <summary>
    /// Logs comprehensive project information and dependency graph after all modules are loaded.
    /// This is shown once at the end of module loading.
    /// </summary>
    private void LogProjectInfoAndGraph(
        string serviceKey,
        string serviceTitle,
        Type startupModuleType,
        IReadOnlyList<BonModuleDescriptor> sortedModules)
    {
        if (_logger == null) return;

        // Ensure service key and title are not null or empty (should be validated earlier, but double-check)
        if (string.IsNullOrWhiteSpace(serviceKey))
        {
            throw new ArgumentException("Service key is required and cannot be null or empty.", nameof(serviceKey));
        }
        
        if (string.IsNullOrWhiteSpace(serviceTitle))
        {
            throw new ArgumentException("Service title is required and cannot be null or empty.", nameof(serviceTitle));
        }

        // Border width constants: total width is 77 characters (1 border char + 75 content + 1 border char)
        const int contentWidth = 75;
        var topBorder = "╔" + new string('═', contentWidth) + "╗";
        var separatorBorder = "╠" + new string('═', contentWidth) + "╣";
        var bottomBorder = "╚" + new string('═', contentWidth) + "╝";
        var emptyLine = "║" + new string(' ', contentWidth) + "║";

        var sb = new System.Text.StringBuilder();
        sb.AppendLine();
        sb.AppendLine(topBorder);
        sb.AppendLine(emptyLine);
        sb.AppendLine(CreateCenteredLine("BONYAN MODULARITY SYSTEM", contentWidth));
        sb.AppendLine(CreateCenteredLine("MODULE LOADING SUMMARY", contentWidth));
        sb.AppendLine(emptyLine);
        sb.AppendLine(separatorBorder);
        
        // Display service title prominently
        var serviceTitleDisplay = $"  SERVICE: {serviceTitle.ToUpperInvariant()}";
        var titlePadding = contentWidth - serviceTitleDisplay.Length;
        sb.AppendLine($"║{serviceTitleDisplay}{new string(' ', Math.Max(0, titlePadding))}║");
        
        // Display service key
        var serviceKeyDisplay = $"  KEY: {serviceKey}";
        var keyPadding = contentWidth - serviceKeyDisplay.Length;
        sb.AppendLine($"║{serviceKeyDisplay}{new string(' ', Math.Max(0, keyPadding))}║");
        
        sb.AppendLine(separatorBorder);
        
        // Module information lines - ensure exact width
        var rootModuleLine = $"  Root Module:        {startupModuleType.Name}";
        var rootModulePadding = contentWidth - rootModuleLine.Length;
        sb.AppendLine($"║{rootModuleLine}{new string(' ', Math.Max(0, rootModulePadding))}║");
        
        var totalModulesLine = $"  Total Modules:      {sortedModules.Count}";
        var totalModulesPadding = contentWidth - totalModulesLine.Length;
        sb.AppendLine($"║{totalModulesLine}{new string(' ', Math.Max(0, totalModulesPadding))}║");
        
        var coreModulesLine = $"  Core Modules:       {sortedModules.Count(m => !m.IsPluginModule)}";
        var coreModulesPadding = contentWidth - coreModulesLine.Length;
        sb.AppendLine($"║{coreModulesLine}{new string(' ', Math.Max(0, coreModulesPadding))}║");
        
        var pluginModulesLine = $"  Plugin Modules:     {sortedModules.Count(m => m.IsPluginModule)}";
        var pluginModulesPadding = contentWidth - pluginModulesLine.Length;
        sb.AppendLine($"║{pluginModulesLine}{new string(' ', Math.Max(0, pluginModulesPadding))}║");
        
        sb.AppendLine(separatorBorder);
        
        // Add load order section with header and border
        sb.AppendLine(CreateCenteredLine("Module Load Order (from root module)", contentWidth));
        sb.AppendLine(separatorBorder);
        var loadOrder = ModuleGraphVisualizer.GenerateLoadOrderVisualization(sortedModules);
        sb.AppendLine(WrapContentInBorder(loadOrder, contentWidth));
        sb.AppendLine(separatorBorder);
        
        // Add dependency graph section with header and border
        sb.AppendLine(CreateCenteredLine("Module Dependency Graph (from root module)", contentWidth));
        sb.AppendLine(separatorBorder);
        var graph = ModuleGraphVisualizer.GenerateTreeVisualization(sortedModules, startupModuleType);
        sb.AppendLine(WrapContentInBorder(graph, contentWidth));
        sb.AppendLine(bottomBorder);
        sb.AppendLine();

        _logger.LogInformation(sb.ToString());
    }

    /// <summary>
    /// Creates a centered line within the border box.
    /// </summary>
    private static string CreateCenteredLine(string text, int contentWidth)
    {
        var padding = contentWidth - text.Length;
        var leftPadding = padding / 2;
        var rightPadding = padding - leftPadding;
        return $"║{new string(' ', leftPadding)}{text}{new string(' ', rightPadding)}║";
    }

    /// <summary>
    /// Wraps multi-line content in borders for better UI/UX.
    /// Each line is wrapped with border characters on both sides.
    /// </summary>
    private static string WrapContentInBorder(string content, int contentWidth)
    {
        if (string.IsNullOrWhiteSpace(content))
            return $"║{new string(' ', contentWidth)}║";

        var lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        var sb = new System.Text.StringBuilder();
        
        foreach (var line in lines)
        {
            // Truncate or pad line to fit content width
            var processedLine = line.Length > contentWidth 
                ? line.Substring(0, contentWidth) 
                : line.PadRight(contentWidth);
            
            sb.AppendLine($"║{processedLine}║");
        }
        
        return sb.ToString().TrimEnd('\r', '\n');
    }

    /// <summary>
    /// Shows information about loaded plugins from JSON manifests (for logging).
    /// </summary>
    private void LogPluginInformation(PlugInSourceList plugInSources)
    {
        var jsonManifests = new List<PluginManifest>();

        // Collect all JSON manifests from different plugin sources
        foreach (var pluginSource in plugInSources)
        {
            if (pluginSource is JsonPluginSource jsonSource)
            {
                jsonManifests.AddRange(jsonSource.Manifests);
            }
            else if (pluginSource is FolderPlugInSource folderSource)
            {
                jsonManifests.AddRange(folderSource.DiscoveredManifests);
            }
        }

        if (jsonManifests.Any())
        {
            _logger?.LogDebug("Discovered {ManifestCount} plugin manifests", jsonManifests.Count);
            foreach (var manifest in jsonManifests)
            {
                _logger?.LogDebug("  Plugin: {PluginName} v{Version} - {Description}",
                    manifest.Name, manifest.Version, manifest.Description ?? "No description");
                if (manifest.Authors.Any())
                {
                    _logger?.LogDebug("    Authors: {Authors}", string.Join(", ", manifest.Authors));
                }
                _logger?.LogDebug("    Entry Point: {EntryPoint}", manifest.EntryPoint);
            }
        }
    }
}
