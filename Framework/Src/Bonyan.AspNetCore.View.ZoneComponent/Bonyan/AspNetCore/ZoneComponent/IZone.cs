namespace Bonyan.AspNetCore.ZoneComponent;

/// <summary>
/// Interface for zone models that provides zone identification and metadata
/// </summary>
public interface IZone
{
    /// <summary>
    /// Gets the unique zone identifier
    /// </summary>
    string ZoneId { get; }

    /// <summary>
    /// Gets the zone name for rendering
    /// </summary>
    string ZoneName { get; }

    /// <summary>
    /// Gets additional metadata for the zone
    /// </summary>
    IDictionary<string, object>? Metadata { get; }
} 