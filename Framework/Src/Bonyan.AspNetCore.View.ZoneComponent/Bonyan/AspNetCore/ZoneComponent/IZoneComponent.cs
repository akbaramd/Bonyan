namespace Bonyan.AspNetCore.ZoneComponent;

/// <summary>
/// Interface for zone components
/// </summary>
public interface IZoneComponent
{
    /// <summary>
    /// Unique identifier for this component instance to prevent duplicates
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Gets the zones this component should be rendered in
    /// </summary>
    IEnumerable<string> Zones { get; }

    /// <summary>
    /// Gets the priority/order for rendering within zones (lower numbers render first)
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// Gets the component parameters definition
    /// </summary>
    ZoneComponentParameters Parameters { get; }

    /// <summary>
    /// Handle the zone component
    /// </summary>
    /// <param name="payload">The payload object</param>
    /// <param name="context">The zone rendering context</param>
    /// <returns>Zone component result</returns>
    Task<ZoneComponentResult> HandleAsync(object payload, ZoneRenderingContext context);
} 