using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Bonyan.Modularity
{
    /// <summary>
    /// Context for configuring services, allowing options setup and configuration binding.
    /// </summary>
    public class BonConfigurationContext : BonContextBase
    {
        public IServiceCollection 
            Services { get; }

        public BonConfigurationContext(IServiceCollection services)
            : base(services.BuildServiceProvider())
        {
            Services = services;
        }

        public BonServiceManager ServiceManager { get; set; }
        

        /// <summary>
        /// Configures options of type <typeparamref name="TOptions"/> using an action.
        /// </summary>
        /// <typeparam name="TOptions">The type of options to configure.</typeparam>
        /// <param name="configureOptions">The action to configure options.</param>
        public void ConfigureOptions<TOptions>(Action<TOptions> configureOptions) where TOptions : class
        {
            Services.Configure(configureOptions);
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
