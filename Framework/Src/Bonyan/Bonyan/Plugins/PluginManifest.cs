using System.Text.Json.Serialization;

namespace Bonyan.Plugins;

/// <summary>
/// Represents the structure of a plugin.json manifest file.
/// </summary>
public class PluginManifest
{
    /// <summary>
    /// The name of the plugin.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The version of the plugin.
    /// </summary>
    [JsonPropertyName("version")]
    public string Version { get; set; } = "1.0.0";

    /// <summary>
    /// Description of the plugin.
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Authors of the plugin.
    /// </summary>
    [JsonPropertyName("authors")]
    public string[] Authors { get; set; } = Array.Empty<string>();

    /// <summary>
    /// The entry point DLL file path relative to the plugin directory.
    /// </summary>
    [JsonPropertyName("entryPoint")]
    public string EntryPoint { get; set; } = string.Empty;

    /// <summary>
    /// Additional DLL files that should be loaded with the plugin.
    /// </summary>
    [JsonPropertyName("additionalFiles")]
    public string[] AdditionalFiles { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Tags/categories for the plugin.
    /// </summary>
    [JsonPropertyName("tags")]
    public string[] Tags { get; set; } = Array.Empty<string>();

    /// <summary>
    /// The directory path where the plugin.json file is located.
    /// </summary>
    [JsonIgnore]
    public string DirectoryPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets the full path to the entry point DLL.
    /// </summary>
    [JsonIgnore]
    public string EntryPointFullPath => Path.Combine(DirectoryPath, EntryPoint);

    /// <summary>
    /// Gets the full paths to all additional DLL files.
    /// </summary>
    [JsonIgnore]
    public string[] AdditionalFilesFullPaths => AdditionalFiles
        .Select(file => Path.Combine(DirectoryPath, file))
        .ToArray();
} 