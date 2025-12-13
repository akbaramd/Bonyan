using Bonyan.DependencyInjection;
using Bonyan.Plugins;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Modularity;

/// <summary>
/// Context for pre-configuration phase.
/// Only allows PreConfigure operations - no Configure or PostConfigure.
/// This ensures PreConfigure is only accessible during OnPreConfigureAsync phase.
/// </summary>
public class BonPreConfigurationContext : BonContextBase
{
    /// <summary>
    /// Gets the service collection for registering services.
    /// </summary>
    public new IServiceCollection Services { get; }

    /// <summary>
    /// Gets or sets the service manager.
    /// </summary>
    public BonServiceManager? ServiceManager { get; set; }

    /// <summary>
    /// Gets the plugin sources list.
    /// </summary>
    public PlugInSourceList PlugInSources { get; } = new PlugInSourceList();

    public BonPreConfigurationContext(IServiceCollection services)
        : base(services.BuildServiceProvider())
    {
        Services = services;
    }

    /// <summary>
    /// Pre-configures options of type <typeparamref name="TOptions"/>.
    /// This method is only available in OnPreConfigureAsync phase.
    /// </summary>
    /// <typeparam name="TOptions">The type of options to pre-configure.</typeparam>
    /// <param name="configureOptions">The action to pre-configure options.</param>
    public void PreConfigure<TOptions>(Action<TOptions> configureOptions) 
        where TOptions : class
    {
        Services.PreConfigure(configureOptions);
    }

    /// <summary>
    /// Gets the list of pre-configure actions for the specified options type.
    /// </summary>
    /// <typeparam name="TOptions">The type of options.</typeparam>
    /// <returns>The list of pre-configure actions.</returns>
    public BonPreConfigureActionList<TOptions> GetPreConfigureActions<TOptions>() 
        where TOptions : class
    {
        return Services.GetPreConfigureActions<TOptions>();
    }
}

