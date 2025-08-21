namespace Bonyan.AspNetCore.ZoneComponent;

/// <summary>
/// Interface for zone registry
/// </summary>
public interface IZoneRegistry
{
    /// <summary>
    /// Add a component to the registry
    /// </summary>
    void Add(IZoneComponent component);

    /// <summary>
    /// Get all components for a specific zone
    /// </summary>
    IEnumerable<IZoneComponent> Get(string zone);

    /// <summary>
    /// Get all components for a specific zone with their metadata
    /// </summary>
    /// <param name="zone">The zone name</param>
    /// <returns>Components with their metadata</returns>

    /// <summary>
    /// Remove a component by its ID from all zones
    /// </summary>
    void Remove(string componentId);

    /// <summary>
    /// Check if a component with the given ID is registered
    /// </summary>
    bool IsRegistered(string componentId);

    /// <summary>
    /// Get all registered component IDs
    /// </summary>
    IEnumerable<string> GetRegisteredComponentIds();



}
