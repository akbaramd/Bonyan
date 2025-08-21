using System.Collections.Concurrent;

namespace Bonyan.AspNetCore.ZoneComponent;

/// <summary>
/// Implementation of zone registry
/// </summary>
public class ZoneRegistry : IZoneRegistry
{
    private readonly ConcurrentDictionary<string, List<IZoneComponent>> _zones = new();
    private readonly HashSet<string> _registeredComponentIds = new();

    public void Add(IZoneComponent component)
    {
        // Check if component with this ID is already registered
        if (_registeredComponentIds.Contains(component.Id))
        {
            Console.WriteLine($"Component with ID '{component.Id}' is already registered. Skipping duplicate.");
            return;
        }

        // Add component ID to registered set
        _registeredComponentIds.Add(component.Id);

        foreach (var zone in component.Zones)
        {
            var zoneComponents = _zones.GetOrAdd(zone, _ => new List<IZoneComponent>());
            
            lock (zoneComponents)
            {
                // Double-check that component is not already in this zone
                if (!zoneComponents.Any(c => c.Id == component.Id))
                {
                    zoneComponents.Add(component);
                    // Sort by priority (lower numbers first)
                    zoneComponents.Sort((a, b) => a.Priority.CompareTo(b.Priority));
                }
            }
        }
    }

    public IEnumerable<IZoneComponent> Get(string zone)
    {
        if (_zones.TryGetValue(zone, out var components))
        {
            return components.ToList(); // Return a copy to avoid modification during enumeration
        }
        
        return Enumerable.Empty<IZoneComponent>();
    }

    /// <summary>
    /// Remove a component by its ID from all zones
    /// </summary>
    public void Remove(string componentId)
    {
        _registeredComponentIds.Remove(componentId);
        
        foreach (var zoneComponents in _zones.Values)
        {
            lock (zoneComponents)
            {
                zoneComponents.RemoveAll(c => c.Id == componentId);
            }
        }
    }

    /// <summary>
    /// Check if a component with the given ID is registered
    /// </summary>
    public bool IsRegistered(string componentId)
    {
        return _registeredComponentIds.Contains(componentId);
    }

    /// <summary>
    /// Get all registered component IDs
    /// </summary>
    public IEnumerable<string> GetRegisteredComponentIds()
    {
        return _registeredComponentIds.ToList();
    }
} 