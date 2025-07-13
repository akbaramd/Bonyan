using Bonyan.Core;
using JetBrains.Annotations;

namespace Bonyan.Plugins;

public static class PlugInSourceListExtensions
{
    public static void AddFolder(
        [NotNull] this PlugInSourceList list,
        [NotNull] string folder,
        bool autoDiscoverJson = true,
        SearchOption searchOption = SearchOption.AllDirectories)
    {
        Check.NotNull(list, nameof(list));

        list.Add(new FolderPlugInSource(folder, searchOption)
        {
            AutoDiscoverJsonManifests = autoDiscoverJson
        });
    }

    public static void AddTypes(
        [NotNull] this PlugInSourceList list,
        params Type[] moduleTypes)
    {
        Check.NotNull(list, nameof(list));

        list.Add(new TypePlugInSource(moduleTypes));
    }

    public static void AddFiles(
        [NotNull] this PlugInSourceList list,
        params string[] filePaths)
    {
        Check.NotNull(list, nameof(list));

        list.Add(new FilePlugInSource(filePaths));
    }

    /// <summary>
    /// Adds a JSON plugin source from plugin.json manifest files.
    /// </summary>
    /// <param name="list">The plugin source list.</param>
    /// <param name="jsonFilePaths">Paths to plugin.json files.</param>
    public static void AddJsonManifests(
        [NotNull] this PlugInSourceList list,
        params string[] jsonFilePaths)
    {
        Check.NotNull(list, nameof(list));

        list.Add(new JsonPluginSource(jsonFilePaths));
    }

    /// <summary>
    /// Adds a single JSON plugin source from a plugin.json manifest file.
    /// </summary>
    /// <param name="list">The plugin source list.</param>
    /// <param name="jsonFilePath">Path to the plugin.json file.</param>
    public static void AddJsonManifest(
        [NotNull] this PlugInSourceList list,
        [NotNull] string jsonFilePath)
    {
        Check.NotNull(list, nameof(list));
        Check.NotNull(jsonFilePath, nameof(jsonFilePath));

        list.Add(new JsonPluginSource(jsonFilePath));
    }

    /// <summary>
    /// Adds a folder plugin source with automatic JSON manifest discovery enabled.
    /// </summary>
    /// <param name="list">The plugin source list.</param>
    /// <param name="folder">The folder path to scan.</param>
    /// <param name="searchOption">The search option for scanning subfolders.</param>
    /// <param name="autoDiscoverJson">Whether to automatically discover plugin.json files.</param>
    public static void AddFolderWithJsonDiscovery(
        [NotNull] this PlugInSourceList list,
        [NotNull] string folder,
        SearchOption searchOption = SearchOption.TopDirectoryOnly,
        bool autoDiscoverJson = true)
    {
        Check.NotNull(list, nameof(list));
        Check.NotNull(folder, nameof(folder));

        var folderSource = new FolderPlugInSource(folder, searchOption)
        {
            AutoDiscoverJsonManifests = autoDiscoverJson
        };

        list.Add(folderSource);
    }

    /// <summary>
    /// Adds multiple folder plugin sources with automatic JSON manifest discovery.
    /// </summary>
    /// <param name="list">The plugin source list.</param>
    /// <param name="folders">The folder paths to scan.</param>
    /// <param name="searchOption">The search option for scanning subfolders.</param>
    /// <param name="autoDiscoverJson">Whether to automatically discover plugin.json files.</param>
    public static void AddFoldersWithJsonDiscovery(
        [NotNull] this PlugInSourceList list,
        IEnumerable<string> folders,
        SearchOption searchOption = SearchOption.TopDirectoryOnly,
        bool autoDiscoverJson = true)
    {
        Check.NotNull(list, nameof(list));
        Check.NotNull(folders, nameof(folders));

        foreach (var folder in folders)
        {
            list.AddFolderWithJsonDiscovery(folder, searchOption, autoDiscoverJson);
        }
    }

    /// <summary>
    /// Scans a directory for plugin.json files and adds them as JSON plugin sources.
    /// </summary>
    /// <param name="list">The plugin source list.</param>
    /// <param name="directory">The directory to scan.</param>
    /// <param name="searchOption">The search option for scanning subfolders.</param>
    public static void ScanForJsonManifests(
        [NotNull] this PlugInSourceList list,
        [NotNull] string directory,
        SearchOption searchOption = SearchOption.AllDirectories)
    {
        Check.NotNull(list, nameof(list));
        Check.NotNull(directory, nameof(directory));

        try
        {
            var fullPath = Path.GetFullPath(directory);
            
            if (!Directory.Exists(fullPath))
            {
                Console.WriteLine($"Directory not found for JSON manifest scan: {fullPath}");
                return;
            }

            var jsonFiles = Directory.GetFiles(fullPath, "plugin.json", searchOption);
            
            if (jsonFiles.Any())
            {
                Console.WriteLine($"Found {jsonFiles.Length} plugin.json files in {directory}");
                list.AddJsonManifests(jsonFiles);
            }
            else
            {
                Console.WriteLine($"No plugin.json files found in {directory}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error scanning for JSON manifests in {directory}: {ex.Message}");
        }
    }
}