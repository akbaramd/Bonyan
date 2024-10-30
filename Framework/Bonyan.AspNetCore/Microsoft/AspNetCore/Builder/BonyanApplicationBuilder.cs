using Bonyan.Modularity.Abstractions;

namespace Microsoft.AspNetCore.Builder
{
    public class BonyanApplicationBuilder : IBonyanApplicationBuilder
    {

        private readonly IWebModularityApplication _modularApp;
        private readonly WebApplicationBuilder _builder;

        public ILoggingBuilder Logging => _builder.Logging;
        public IServiceCollection Services => _builder.Services;
        public IHostBuilder Host => _builder.Host;
        public IConfigurationManager Configuration => _builder.Configuration;
        public IHostEnvironment Environment => _builder.Environment;


        public BonyanApplicationBuilder(IWebModularityApplication modularApp, WebApplicationBuilder builder)
        {
            _modularApp = modularApp;
            _builder = builder;
        }


        public WebApplication Build()
        {
            var application = _builder.Build();
            _modularApp.InitializeModulesAsync(application.Services).GetAwaiter().GetResult();
            _modularApp.InitializeApplicationAsync(application).GetAwaiter().GetResult();
            return application;
        }
    }
}