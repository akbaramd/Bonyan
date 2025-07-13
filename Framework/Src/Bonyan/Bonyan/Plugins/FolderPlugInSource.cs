using System.Reflection;
using System.Runtime.Loader;
using Bonyan.Core;
using Bonyan.Modularity.Abstractions;
using Bonyan.Reflection;
using JetBrains.Annotations;

namespace Bonyan.Plugins;

public class FolderPlugInSource : IPlugInSource
{
    public string Folder { get; }

    public SearchOption SearchOption { get; set; }

    public Func<string, bool>? Filter { get; set; }

    /// <summary>
    /// Gets or sets whether to automatically discover plugin.json files.
    /// </summary>
    public bool AutoDiscoverJsonManifests { get; set; } = true;

    private readonly List<PluginManifest> _discoveredManifests = new();

    /// <summary>
    /// Gets the discovered plugin manifests.
    /// </summary>
    public IReadOnlyList<PluginManifest> DiscoveredManifests => _discoveredManifests.AsReadOnly();

    public FolderPlugInSource(
        [NotNull] string folder,
        SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        Check.NotNull(folder, nameof(folder));

        Folder = folder;
        SearchOption = searchOption;
    }

    public Type[] GetModules()
    {
        var modules = new List<Type>();

        // First, try to discover and load modules from plugin.json files
        if (AutoDiscoverJsonManifests)
        {
            var jsonPluginModules = GetModulesFromJsonManifests();
            modules.AddRange(jsonPluginModules);
        }

        // Then, load modules from DLL files (existing behavior)
        var dllPluginModules = GetModulesFromDllFiles();
        modules.AddRange(dllPluginModules);

        return modules.ToArray();
    }

    /// <summary>
    /// Gets modules from plugin.json manifest files.
    /// </summary>
    private Type[] GetModulesFromJsonManifests()
    {
        var modules = new List<Type>();
        var jsonFiles = GetJsonManifestFiles();

        if (jsonFiles.Any())
        {
            Console.WriteLine($"Discovered {jsonFiles.Count()} plugin.json files in {Folder}");
            
            var jsonPluginSource = new JsonPluginSource(jsonFiles.ToArray());
            _discoveredManifests.AddRange(jsonPluginSource.Manifests);
            
            modules.AddRange(jsonPluginSource.GetModules());
        }

        return modules.ToArray();
    }

    /// <summary>
    /// Gets modules from DLL files (original behavior).
    /// </summary>
    private Type[] GetModulesFromDllFiles()
    {
        var modules = new List<Type>();

        foreach (var assembly in GetAssemblies())
        {
            try
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (BonModule.IsBonyanModule(type))
                    {
                        modules.AddIfNotContains(type);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log warning but continue processing other assemblies
                Console.WriteLine($"Warning: Could not get module types from assembly {assembly.FullName}: {ex.Message}");
            }
        }

        return modules.ToArray();
    }

    /// <summary>
    /// Gets all plugin.json files from the folder.
    /// </summary>
    private IEnumerable<string> GetJsonManifestFiles()
    {
        try
        {
            var fullFolderPath = Path.GetFullPath(Folder);
            
            if (!Directory.Exists(fullFolderPath))
            {
                Console.WriteLine($"Plugin folder not found: {fullFolderPath}");
                return Enumerable.Empty<string>();
            }

            return Directory
                .EnumerateFiles(fullFolderPath, "plugin.json", SearchOption)
                .Where(file => Filter?.Invoke(file) ?? true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Could not search for plugin.json files in {Folder}: {ex.Message}");
            return Enumerable.Empty<string>();
        }
    }

    private List<Assembly> GetAssemblies()
    {
        var assemblyFiles = AssemblyHelper.GetAssemblyFiles(Folder, SearchOption);

        if (Filter != null)
        {
            assemblyFiles = assemblyFiles.Where(Filter);
        }

        return assemblyFiles
            .Select(Path.GetFullPath) // Convert to absolute path
            .Select(AssemblyLoadContext.Default.LoadFromAssemblyPath)
            .ToList();
    }
}
