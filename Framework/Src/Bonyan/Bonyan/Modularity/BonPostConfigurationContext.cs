using Bonyan.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Bonyan.Modularity;

/// <summary>
/// Context for post-configuration phase.
/// Only allows PostConfigure operations - no PreConfigure or Configure.
/// This ensures PostConfigure is only accessible during OnPostConfigureAsync phase.
/// </summary>
public class BonPostConfigurationContext : BonContextBase
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

    public BonPostConfigurationContext(IServiceCollection services)
        : base(services.BuildServiceProvider())
    {
        Services = services;
    }

    /// <summary>
    /// Post-configures options of type <typeparamref name="TOptions"/>.
    /// This method is only available in OnPostConfigureAsync phase.
    /// </summary>
    /// <typeparam name="TOptions">The type of options to post-configure.</typeparam>
    /// <param name="configureOptions">The action to post-configure options.</param>
    public void PostConfigure<TOptions>(Action<TOptions> configureOptions) 
        where TOptions : class
    {
        Services.PostConfigure(configureOptions);
    }

    /// <summary>
    /// Post-configures all instances of options of type <typeparamref name="TOptions"/>.
    /// </summary>
    /// <typeparam name="TOptions">The type of options to post-configure.</typeparam>
    /// <param name="configureOptions">The action to post-configure options.</param>
    public void PostConfigureAll<TOptions>(Action<TOptions> configureOptions) 
        where TOptions : class
    {
        Services.PostConfigureAll(configureOptions);
    }

    /// <summary>
    /// Executes all pre-configured actions and returns the configured options instance.
    /// This is useful when you need to read pre-configured options in the post-configure phase.
    /// </summary>
    /// <typeparam name="TOptions">The type of options.</typeparam>
    /// <param name="options">The options instance to configure.</param>
    /// <returns>The configured options instance.</returns>
    public TOptions ExecutePreConfiguredActions<TOptions>(TOptions options) 
        where TOptions : class
    {
        return Services.ExecutePreConfiguredActions(options);
    }

    /// <summary>
    /// Executes all pre-configured actions and returns a new configured options instance.
    /// </summary>
    /// <typeparam name="TOptions">The type of options.</typeparam>
    /// <returns>A new configured options instance.</returns>
    public TOptions ExecutePreConfiguredActions<TOptions>() 
        where TOptions : class, new()
    {
        return Services.ExecutePreConfiguredActions<TOptions>();
    }
}

