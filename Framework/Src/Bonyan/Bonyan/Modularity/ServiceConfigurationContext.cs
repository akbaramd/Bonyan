using Bonyan.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Bonyan.Modularity
{
    /// <summary>
    /// Context for main configuration phase.
    /// Only allows Configure operations - no PreConfigure or PostConfigure.
    /// This ensures Configure is only accessible during OnConfigureAsync phase.
    /// </summary>
    public class BonConfigurationContext : BonContextBase
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

        public BonConfigurationContext(IServiceCollection services)
            : base(services.BuildServiceProvider())
        {
            Services = services;
        }

        /// <summary>
        /// Configures options of type <typeparamref name="TOptions"/> using an action.
        /// This method is only available in OnConfigureAsync phase.
        /// </summary>
        /// <typeparam name="TOptions">The type of options to configure.</typeparam>
        /// <param name="configureOptions">The action to configure options.</param>
        public void ConfigureOptions<TOptions>(Action<TOptions> configureOptions) where TOptions : class
        {
            Services.Configure(configureOptions);
        }

        /// <summary>
        /// Executes all pre-configured actions and returns the configured options instance.
        /// This is useful when you need to read pre-configured options in the configure phase.
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

        /// <summary>
        /// Configures options of type <typeparamref name="TOptions"/> and validates them using a validation function.
        /// </summary>
        /// <typeparam name="TOptions">The type of options to configure and validate.</typeparam>
        /// <param name="configureOptions">The action to configure options.</param>
        /// <param name="validate">The function to validate options. Returns true if valid; otherwise, false.</param>
        public void ConfigureAndValidate<TOptions>(Action<TOptions> configureOptions, Func<TOptions, bool> validate) where TOptions : class, new()
        {
            Services.Configure<TOptions>(configureOptions);
            Services.PostConfigure<TOptions>(options =>
            {
                if (!validate(options))
                {
                    throw new OptionsValidationException(typeof(TOptions).Name, typeof(TOptions), new[] { "Validation failed for configured options." });
                }
            });
        }
        

        /// <summary>
        /// Configures named options of type <typeparamref name="TOptions"/>.
        /// </summary>
        /// <typeparam name="TOptions">The type of options to configure.</typeparam>
        /// <param name="name">The name of the options instance to configure.</param>
        /// <param name="configureOptions">The action to configure the named options.</param>
        public void ConfigureNamedOptions<TOptions>(string name, Action<TOptions> configureOptions) where TOptions : class
        {
            Services.Configure(name, configureOptions);
        }

        /// <summary>
        /// Configures and validates named options of type <typeparamref name="TOptions"/>.
        /// </summary>
        /// <typeparam name="TOptions">The type of options to configure and validate.</typeparam>
        /// <param name="name">The name of the options instance to configure.</param>
        /// <param name="configureOptions">The action to configure the named options.</param>
        /// <param name="validate">The function to validate options. Returns true if valid; otherwise, false.</param>
        public void ConfigureAndValidateNamedOptions<TOptions>(string name, Action<TOptions> configureOptions, Func<TOptions, bool> validate) where TOptions : class, new()
        {
            Services.Configure(name, configureOptions);
            Services.PostConfigure<TOptions>(name, options =>
            {
                if (!validate(options))
                {
                    throw new OptionsValidationException(typeof(TOptions).Name, typeof(TOptions), new[] { $"Validation failed for named options '{name}'." });
                }
            });
        }
    }
}
