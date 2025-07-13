using System.Runtime.Loader;
using System.Text.Json;
using Bonyan.Exceptions;
using Bonyan.Modularity.Abstractions;
using JetBrains.Annotations;

namespace Bonyan.Plugins;

/// <summary>
/// A plugin source that loads plugins from JSON manifest files.
/// </summary>
public class JsonPluginSource : IPlugInSource
{
    private readonly string[] _jsonFilePaths;
    private readonly List<PluginManifest> _manifests = new();

    /// <summary>
    /// Gets the loaded plugin manifests.
    /// </summary>
    public IReadOnlyList<PluginManifest> Manifests => _manifests.AsReadOnly();

    /// <summary>
    /// Initializes a new instance of JsonPluginSource with JSON file paths.
    /// </summary>
    /// <param name="jsonFilePaths">Paths to plugin.json files.</param>
    public JsonPluginSource(params string[] jsonFilePaths)
    {
        _jsonFilePaths = jsonFilePaths ?? Array.Empty<string>();
        LoadManifests();
    }

    /// <summary>
    /// Gets all module types from the plugins defined in JSON manifests.
    /// </summary>
    /// <returns>Array of module types found in the plugin assemblies.</returns>
    [NotNull]
    public Type[] GetModules()
    {
        var modules = new List<Type>();

        foreach (var manifest in _manifests)
        {
            try
            {
                var assemblies = LoadPluginAssemblies(manifest);
                
                foreach (var assembly in assemblies)
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
                        Console.WriteLine($"Warning: Could not get module types from assembly {assembly.FullName} in plugin {manifest.Name}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new BonException($"Could not load plugin '{manifest.Name}' from manifest: {ex.Message}", ex);
            }
        }

        return modules.ToArray();
    }

    /// <summary>
    /// Loads plugin manifests from JSON files.
    /// </summary>
    private void LoadManifests()
    {
        foreach (var jsonFilePath in _jsonFilePaths)
        {
            try
            {
                var fullPath = Path.GetFullPath(jsonFilePath);
                var directoryPath = Path.GetDirectoryName(fullPath) ?? string.Empty;
                
                if (!File.Exists(fullPath))
                {
                    Console.WriteLine($"Warning: Plugin manifest file not found: {fullPath}");
                    continue;
                }

                var json = File.ReadAllText(fullPath);
                var manifest = JsonSerializer.Deserialize<PluginManifest>(json);
                
                if (manifest != null)
                {
                    manifest.DirectoryPath = directoryPath;
                    _manifests.Add(manifest);
                    
                    Console.WriteLine($"Loaded plugin manifest: {manifest.Name} v{manifest.Version}");
                    if (manifest.Authors.Any())
                    {
                        Console.WriteLine($"  Authors: {string.Join(", ", manifest.Authors)}");
                    }
                    if (!string.IsNullOrEmpty(manifest.Description))
                    {
                        Console.WriteLine($"  Description: {manifest.Description}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not load plugin manifest from {jsonFilePath}: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Loads all assemblies (entry point and additional files) for a plugin.
    /// </summary>
    /// <param name="manifest">The plugin manifest.</param>
    /// <returns>List of loaded assemblies.</returns>
    private List<System.Reflection.Assembly> LoadPluginAssemblies(PluginManifest manifest)
    {
        var assemblies = new List<System.Reflection.Assembly>();

        // Load entry point assembly
        if (!string.IsNullOrEmpty(manifest.EntryPoint))
        {
            var entryPointPath = Path.GetFullPath(manifest.EntryPointFullPath);
            if (File.Exists(entryPointPath))
            {
                var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(entryPointPath);
                assemblies.Add(assembly);
                Console.WriteLine($"  Loaded entry point: {Path.GetFileName(entryPointPath)}");
            }
            else
            {
                throw new FileNotFoundException($"Entry point file not found: {entryPointPath}");
            }
        }

        // Load additional assemblies
        foreach (var additionalFile in manifest.AdditionalFiles)
        {
            var additionalPath = Path.GetFullPath(Path.Combine(manifest.DirectoryPath, additionalFile));
            if (File.Exists(additionalPath))
            {
                var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(additionalPath);
                assemblies.Add(assembly);
                Console.WriteLine($"  Loaded additional file: {Path.GetFileName(additionalPath)}");
            }
            else
            {
                Console.WriteLine($"  Warning: Additional file not found: {additionalPath}");
            }
        }

        return assemblies;
    }
} 