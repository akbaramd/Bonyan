namespace Bonyan.Modularity;

/// <summary>
/// Exception thrown when a module phase execution fails.
/// Provides context about which phase and module failed.
/// </summary>
public class ModulePhaseException : InvalidOperationException
{
    /// <summary>
    /// Gets the name of the phase that failed.
    /// </summary>
    public string PhaseName { get; }

    /// <summary>
    /// Gets the type of the module that failed.
    /// </summary>
    public Type ModuleType { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="ModulePhaseException"/>.
    /// </summary>
    /// <param name="phaseName">The name of the phase that failed.</param>
    /// <param name="moduleType">The type of the module that failed.</param>
    /// <param name="innerException">The exception that caused this failure.</param>
    public ModulePhaseException(string phaseName, Type moduleType, Exception innerException)
        : base(
            $"Module phase '{phaseName}' failed for module '{moduleType.FullName}'. " +
            $"See inner exception for details.",
            innerException)
    {
        PhaseName = phaseName ?? throw new ArgumentNullException(nameof(phaseName));
        ModuleType = moduleType ?? throw new ArgumentNullException(nameof(moduleType));
    }
}

