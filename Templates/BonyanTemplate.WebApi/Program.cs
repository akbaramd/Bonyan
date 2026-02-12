using Bonyan.Plugins;
using BonyanTemplate.WebApi;

var builder = BonyanApplication.CreateModularBuilder<BonyanTemplateWebApiModule>(
    "BonyanTemplate",
    "Bonyan Template Web API",
    _ => { });

var app = await builder.BuildAsync();
await app.RunAsync();
