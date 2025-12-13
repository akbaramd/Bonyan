namespace Bonyan.Modularity.Abstractions;

/// <summary>
/// Defines the configuration lifecycle for modules.
/// Part of the microkernel core architecture.
/// Each phase has its own context with restricted access to appropriate methods.
/// </summary>
public interface IConfigurableModule
{
    /// <summary>
    /// Called before configuration phase begins.
    /// Only PreConfigure operations are available in this phase.
    /// </summary>
    ValueTask OnPreConfigureAsync(BonPreConfigurationContext context, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Called during the main configuration phase.
    /// Only Configure operations are available in this phase.
    /// </summary>
    ValueTask OnConfigureAsync(BonConfigurationContext context, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Called after configuration phase completes.
    /// Only PostConfigure operations are available in this phase.
    /// </summary>
    ValueTask OnPostConfigureAsync(BonPostConfigurationContext context, CancellationToken cancellationToken = default);
}
