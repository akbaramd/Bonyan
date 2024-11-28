namespace Bonyan;

public class BonServiceManager
{
    public string ServiceId { get; set; } = string.Empty;

    private readonly Dictionary<string, BonFeatureHealth> _moduleHealth = new();

    /// <summary>
    /// Registers a module's health information.
    /// </summary>
    /// <param name="moduleName">The name of the module.</param>
    /// <param name="statusProvider">A function that provides the current health status of the module.</param>
    public void RegisterFeatureHealth(string moduleName, Func<BonHealthStatus> statusProvider)
    {
        if (string.IsNullOrWhiteSpace(moduleName))
            throw new ArgumentException("Module name cannot be null or empty.", nameof(moduleName));

        if (statusProvider == null)
            throw new ArgumentNullException(nameof(statusProvider));

        _moduleHealth[moduleName] = new BonFeatureHealth(statusProvider);
    }

    /// <summary>
    /// Gets the health status of all registered modules.
    /// </summary>
    /// <returns>A dictionary of module names and their health statuses.</returns>
    public IReadOnlyDictionary<string, BonHealthStatus> GetFeatureHealthStatuses()
    {
        var statuses = new Dictionary<string, BonHealthStatus>();

        foreach (var (moduleName, moduleHealth) in _moduleHealth)
        {
            statuses[moduleName] = moduleHealth.GetStatus();
        }

        return statuses;
    }

    private class BonFeatureHealth
    {
        private readonly Func<BonHealthStatus> _statusProvider;

        public BonFeatureHealth(Func<BonHealthStatus> statusProvider)
        {
            _statusProvider = statusProvider;
        }

        public BonHealthStatus GetStatus() => _statusProvider();
    }
}

public enum BonHealthStatus
{
    Up,
    Down,
    Degraded
}

