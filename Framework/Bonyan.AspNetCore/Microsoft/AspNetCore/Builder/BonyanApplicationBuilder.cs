
using System;
using Bonyan.AspNetCore.Context;
using Bonyan.AspNetCore.Domain;
using Bonyan.AspNetCore.Infrastructure;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;

namespace Microsoft.AspNetCore.Builder
{
    public class BonyanApplicationBuilder : IBonyanApplicationBuilder
    {
        private readonly List<ConsoleMessage> _consoleMessages = new();
        
      
        private readonly WebApplicationBuilder _builder;
        private readonly IBonyanContext _context;

        public BonyanServiceInfo ServiceInfo { get; }
        public IServiceCollection Services { get; }
        public ConfigureHostBuilder Host { get; }
        public IConfiguration Configuration { get; }
        
        
        public BonyanApplicationBuilder(BonyanServiceInfo serviceInfo, WebApplicationBuilder builder)
        {
            ServiceInfo = serviceInfo;
            Services = builder.Services;
            Host = builder.Host;
            Configuration = builder.Configuration;
            _builder = builder;
            
             _context = new BonyanContext(this);
        }

      

        
        public BonyanApplication Build()
        {
            

            var application = _builder.Build();
            
            var bonyanApplication = new BonyanApplication(application, ServiceInfo) { ConsoleMessages = _consoleMessages };
           
            var webModularityApplication = application.Services.GetRequiredService<IWebModularityApplication>();
            
            _context.Build(bonyanApplication);
            
            webModularityApplication.InitializeAsync().GetAwaiter().GetResult();
            
      

            webModularityApplication.ApplicationAsync(new ModularityApplicationContext(bonyanApplication)).GetAwaiter().GetResult();
            
            
            return bonyanApplication;
        }
    }
}
