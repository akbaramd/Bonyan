using Bonyan.Modularity.Abstractions;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Builds a modular ASP.NET Core application with Bonyan modularity.
    /// </summary>
    public class BonyanApplicationBuilder : IBonyanApplicationBuilder
    {
        private readonly IWebBonModularityApplication _modularApp;
        private readonly WebApplicationBuilder _builder;

        /// <summary>
        /// Provides access to logging services.
        /// </summary>
        public ILoggingBuilder Logging => _builder.Logging;

        /// <summary>
        /// Provides access to dependency injection services.
        /// </summary>
        public IServiceCollection Services => _builder.Services;

        /// <summary>
        /// Provides access to the host configuration.
        /// </summary>
        public IHostBuilder Host => _builder.Host;

        /// <summary>
        /// Provides access to configuration settings.
        /// </summary>
        public IConfigurationManager Configuration => _builder.Configuration;

        /// <summary>
        /// Provides access to environment-specific settings.
        /// </summary>
        public IHostEnvironment Environment => _builder.Environment;

        /// <summary>
        /// Initializes a new instance of the <see cref="BonyanApplicationBuilder"/> class.
        /// </summary>
        /// <param name="modularApp">The modular application interface for module management.</param>
        /// <param name="builder">The underlying WebApplicationBuilder instance.</param>
        public BonyanApplicationBuilder(IWebBonModularityApplication modularApp, WebApplicationBuilder builder)
        {
            _modularApp = modularApp;
            _builder = builder;
        }

        /// <summary>
        /// Builds and initializes the modular ASP.NET Core application.
        /// </summary>
        /// <returns>Returns a fully built and initialized WebApplication instance.</returns>
        public async Task RunAsync()
        {
            var application = _builder.Build();

            // Asynchronously initialize modules and application, with error handling.
            try
            {
                await _modularApp.InitializeModulesAsync(application.Services);
                await _modularApp.InitializeApplicationAsync(application);
            }
            catch (Exception ex)
            {
                // Log or handle initialization errors
                throw new ApplicationException("An error occurred during module initialization.", ex);
            }

            await application.RunAsync();
        }
    }
}
