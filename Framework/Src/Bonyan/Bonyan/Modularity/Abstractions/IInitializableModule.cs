namespace Bonyan.Modularity.Abstractions;

/// <summary>
/// Defines the initialization lifecycle for modules.
/// Part of the microkernel core architecture.
/// </summary>
public interface IInitializableModule
{
    /// <summary>
    /// Called before initialization phase begins.
    /// </summary>
    ValueTask OnPreInitializeAsync(BonInitializedContext context, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Called during the main initialization phase.
    /// </summary>
    ValueTask OnInitializeAsync(BonInitializedContext context, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Called after initialization phase completes.
    /// </summary>
    ValueTask OnPostInitializeAsync(BonInitializedContext context, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Called during application shutdown for clean disposal.
    /// </summary>
    ValueTask OnShutdownAsync(BonShutdownContext context, CancellationToken cancellationToken = default);
}
