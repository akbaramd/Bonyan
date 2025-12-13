using System.Reflection;
using System.Runtime.Loader;

namespace Bonyan.ArchitectureTests;

/// <summary>
/// Discovers and loads Bonyan assemblies for architecture testing.
/// </summary>
internal static class AssemblyDiscovery
{
    /// <summary>
    /// Loads all Bonyan assemblies from the test output directory.
    /// </summary>
    /// <returns>List of loaded assemblies.</returns>
    public static IReadOnlyList<Assembly> LoadBonyanAssemblies()
    {
        // Get the directory containing the test assembly
        var testAssemblyLocation = typeof(AssemblyDiscovery).Assembly.Location;
        var testDir = Path.GetDirectoryName(testAssemblyLocation);
        
        if (string.IsNullOrEmpty(testDir))
        {
            throw new InvalidOperationException("Cannot determine test assembly directory.");
        }

        // Look for Bonyan assemblies in the test output directory
        var dlls = Directory.EnumerateFiles(testDir, $"{ArchitectureTestConstants.CompanyPrefix}*.dll", SearchOption.TopDirectoryOnly)
            .Where(f => !f.Contains(".Tests") && !f.Contains(".Test")) // Exclude test assemblies
            .ToList();

        var loaded = new Dictionary<string, Assembly>(StringComparer.OrdinalIgnoreCase);
        var loadContext = AssemblyLoadContext.Default;

        foreach (var path in dlls)
        {
            try
            {
                var fileName = Path.GetFileNameWithoutExtension(path);
                
                // Skip if already loaded
                if (loaded.ContainsKey(fileName))
                    continue;

                // Skip test assemblies
                if (fileName.Contains(".Tests", StringComparison.OrdinalIgnoreCase) ||
                    fileName.Contains(".Test", StringComparison.OrdinalIgnoreCase))
                    continue;

                var asm = loadContext.LoadFromAssemblyPath(path);
                loaded[fileName] = asm;
            }
            catch (Exception ex)
            {
                // Log but continue - some assemblies may fail to load
                System.Diagnostics.Debug.WriteLine($"Failed to load assembly {path}: {ex.Message}");
            }
        }

        return loaded.Values.ToList();
    }

    /// <summary>
    /// Loads assemblies from a specific directory path.
    /// </summary>
    /// <param name="directoryPath">Directory to search for assemblies.</param>
    /// <returns>List of loaded assemblies.</returns>
    public static IReadOnlyList<Assembly> LoadBonyanAssembliesFrom(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            throw new DirectoryNotFoundException($"Directory not found: {directoryPath}");
        }

        var dlls = Directory.EnumerateFiles(directoryPath, $"{ArchitectureTestConstants.CompanyPrefix}*.dll", SearchOption.TopDirectoryOnly)
            .Where(f => !f.Contains(".Tests") && !f.Contains(".Test"))
            .ToList();

        var loaded = new Dictionary<string, Assembly>(StringComparer.OrdinalIgnoreCase);
        var loadContext = AssemblyLoadContext.Default;

        foreach (var path in dlls)
        {
            try
            {
                var fileName = Path.GetFileNameWithoutExtension(path);
                if (loaded.ContainsKey(fileName))
                    continue;

                var asm = loadContext.LoadFromAssemblyPath(path);
                loaded[fileName] = asm;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load assembly {path}: {ex.Message}");
            }
        }

        return loaded.Values.ToList();
    }
}

