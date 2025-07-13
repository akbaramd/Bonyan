using Blazimum.Demo;
using Bonyan.Plugins;

var builder = BonyanApplication.CreateModularBuilder<BlazimumDemoModule>("demo-app", c =>
{
    c.PlugInSources.AddFolder("./Plugins");
},args: args);

var app = await builder.BuildAsync();

await app.RunAsync();