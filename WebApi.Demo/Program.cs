using Bonyan.AspNetCore;
using Bonyan.Modularity;
using WebApi.Demo.Modules;

// Create the modular application builder with fluent API
var builder = BonyanApplication.CreateModularBuilder<WebApiDemoModule>(
    serviceKey: "web-api-demo",
    serviceTitle: "Web API Demo Application"
       
);

// Build the application
// Web application pipeline is configured in WebApiDemoModule.OnApplicationAsync
var app = await builder.BuildAsync();

// Run the application
await app.RunAsync();

