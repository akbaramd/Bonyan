using System;
using Bonyan.AspNetCore.Context;
using Bonyan.AspNetCore.Domain;
using Bonyan.AspNetCore.Infrastructure;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;

namespace Microsoft.AspNetCore.Builder
{
    public class BonyanApplicationBuilder :  IBonyanApplicationBuilder
    {
        private readonly List<ConsoleMessage> _consoleMessages = new();


        private readonly WebApplicationBuilder _builder;

        public BonyanServiceInfo ServiceInfo { get; }
        public ILoggingBuilder Logging => _builder.Logging;
        public IServiceCollection Services => _builder.Services;
        public ConfigureHostBuilder Host => _builder.Host;
        public IConfigurationManager Configuration => _builder.Configuration;
        public IHostEnvironment Environment => _builder.Environment;


        public BonyanApplicationBuilder(BonyanServiceInfo serviceInfo, WebApplicationBuilder builder)
        {
            ServiceInfo = serviceInfo;
            _builder = builder;
        }


        public WebApplication Build()
        {
            var application = _builder.Build();


            var webModularityApplication = application.Services.GetRequiredService<IWebModularityApplication>();


            webModularityApplication.InitializeAsync().GetAwaiter().GetResult();


            webModularityApplication.ApplicationAsync(new ModularityApplicationContext(application)).GetAwaiter()
                .GetResult();


            return application;
        }
    }
}