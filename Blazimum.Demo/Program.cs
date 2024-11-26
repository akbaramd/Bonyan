using Blazimum.Demo;
using Blazimum.Demo.Components;

var builder = BonyanApplication.CreateModularBuilder<BlazimumDemoModule>("demo", args: args);

var app = await builder.BuildAsync();

await app.RunAsync();