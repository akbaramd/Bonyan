using Bonyan.Modularity.Abstractions;
using Microsoft.Extensions.Logging;

namespace Bonyan.Modularity;

/// <summary>
/// Manages the initialization of web modules for a modular application, executing each phase in sequence.
/// Part of the microkernel core - orchestrates plugin (module) lifecycle.
/// </summary>
/// <typeparam name="TModule">The root module type.</typeparam>
public class WebBonModularityApplication<TModule> : BonModularityApplication<TModule>, IWebBonModularityApplication
    where TModule : IBonModule
{
    private readonly ILogger<WebBonModularityApplication<TModule>>? _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="WebBonModularityApplication{TModule}"/> class.
    /// </summary>
    /// <param name="serviceCollection">Service collection to register dependencies.</param>
    /// <param name="serviceKey">The service key (unique identifier). Required.</param>
    /// <param name="serviceTitle">The service title (display name). Required.</param>
    /// <param name="creationContext">Optional creation context.</param>
    /// <param name="logger">Optional logger.</param>
    public WebBonModularityApplication(
        IServiceCollection serviceCollection,
        string serviceKey,
        string serviceTitle,
        Action<BonConfigurationContext>? creationContext = null,
        ILogger<WebBonModularityApplication<TModule>>? logger = null)
        : base(serviceCollection, serviceKey, serviceTitle, creationContext)
    {
        _logger = logger;
    }

    /// <summary>
    /// Asynchronously initializes the web application by executing the pre-application, main application,
    /// and post-application phases for all loaded web modules.
    /// </summary>
    /// <param name="application">The <see cref="WebApplication"/> instance.</param>
    /// <param name="applicationContext">Optional action to configure the application context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="ArgumentNullException">Thrown if application is null.</exception>
    /// <exception cref="ModulePhaseException">Thrown if a module phase fails.</exception>
    public async Task InitializeApplicationAsync(
        WebApplication application,
        Action<BonWebApplicationContext>? applicationContext = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(application);

        _logger?.LogInformation("Initializing web application with {ModuleCount} web modules", 
            Modules.Count(m => m.Instance is IWebModule));

        var ctx = new BonWebApplicationContext(application);
        applicationContext?.Invoke(ctx);

        var webModules = Modules
            .Where(m => m.Instance != null)
            .Select(m => m.Instance!)
            .OfType<IWebModule>()
            .ToList();

        if (webModules.Count == 0)
        {
            _logger?.LogWarning("No web modules found. Application may not function correctly.");
            return;
        }

        // Execute each phase with proper async/await (no deadlock risk)
        await ExecutePhaseAsync(webModules, m => m.OnPreApplicationAsync(ctx, cancellationToken), "PreApplication", cancellationToken);
        await ExecutePhaseAsync(webModules, m => m.OnApplicationAsync(ctx, cancellationToken), "Application", cancellationToken);
        await ExecutePhaseAsync(webModules, m => m.OnPostApplicationAsync(ctx, cancellationToken), "PostApplication", cancellationToken);

        _logger?.LogInformation("Web application initialization completed successfully");
    }

    /// <summary>
    /// Executes a specified phase for each module with detailed error handling and logging.
    /// </summary>
    /// <param name="modules">The list of modules to initialize.</param>
    /// <param name="phaseAction">The phase action to execute for each module.</param>
    /// <param name="phaseName">The name of the phase for logging and error tracking.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    private async Task ExecutePhaseAsync(
        IReadOnlyList<IWebModule> modules,
        Func<IWebModule, ValueTask> phaseAction,
        string phaseName,
        CancellationToken cancellationToken)
    {
        _logger?.LogDebug("Executing phase '{PhaseName}' for {ModuleCount} modules", phaseName, modules.Count);

        foreach (var module in modules)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var moduleType = module.GetType();
            _logger?.LogDebug("Phase {PhaseName} -> Module {ModuleType}", phaseName, moduleType.FullName);

            try
            {
                await phaseAction(module);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, 
                    "Module phase '{PhaseName}' failed for module '{ModuleType}'", 
                    phaseName, moduleType.FullName);
                
                throw new ModulePhaseException(phaseName, moduleType, ex);
            }
        }

        _logger?.LogDebug("Phase '{PhaseName}' completed successfully", phaseName);
    }
}