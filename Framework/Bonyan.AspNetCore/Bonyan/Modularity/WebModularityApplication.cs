using Bonyan.Modularity.Abstractions;

namespace Bonyan.Modularity;

/// <summary>
/// Manages the initialization of web modules for a modular application, executing each phase in sequence.
/// </summary>
/// <typeparam name="TModule">The root module type.</typeparam>
public class WebBonModularityApplication<TModule> : BonModularityApplication<TModule>, IWebBonModularityApplication 
    where TModule : IBonModule
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WebBonModularityApplication{TModule}"/> class.
    /// </summary>
    /// <param name="serviceCollection">Service collection to register dependencies.</param>
    public WebBonModularityApplication(IServiceCollection serviceCollection) : base(serviceCollection)
    {
    }

    /// <summary>
    /// Asynchronously initializes the web application by executing the pre-application, main application,
    /// and post-application phases for all loaded web modules.
    /// </summary>
    /// <param name="application">The <see cref="WebApplication"/> instance.</param>
    /// <exception cref="InvalidOperationException">Thrown if module initialization fails.</exception>
    public async Task InitializeApplicationAsync(WebApplication application)
    {
        if (application == null)
            throw new ArgumentNullException(nameof(application), "Application cannot be null.");

        var webModules = Modules.Select(x => x.Instance)
                                .OfType<IWebModule>()
                                .ToList();

        var context = new BonContext(application);

        // Execute each phase with error handling
        await ExecuteModulePhaseAsync(webModules, (module) => module.OnPreApplicationAsync(context), "Pre-Application");
        await ExecuteModulePhaseAsync(webModules, (module) => module.OnApplicationAsync(context), "Application");
        await ExecuteModulePhaseAsync(webModules, (module) => module.OnPostApplicationAsync(context), "Post-Application");
    }

    /// <summary>
    /// Executes a specified phase for each module with detailed error handling.
    /// </summary>
    /// <param name="modules">The list of modules to initialize.</param>
    /// <param name="phaseAction">The phase action to execute for each module.</param>
    /// <param name="phaseName">The name of the phase for logging and error tracking purposes.</param>
    private static async Task ExecuteModulePhaseAsync(
        IEnumerable<IWebModule> modules,
        Func<IWebModule, Task> phaseAction,
        string phaseName)
    {
        foreach (var module in modules)
        {
            try
            {
                await phaseAction(module);
            }
            catch (Exception ex)
            {
                // Log the error or handle it according to your application's needs
                // Example: logger.LogError(ex, $"Error during {phaseName} phase for module {module.GetType().Name}");
                throw new InvalidOperationException(
                    $"An error occurred during the {phaseName} phase for module '{module.GetType().Name}'.", ex);
            }
        }
    }
}
