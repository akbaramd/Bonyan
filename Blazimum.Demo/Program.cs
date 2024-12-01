using Blazimum.Demo;

var builder = BonyanApplication.CreateModularBuilder<BlazimumDemoModule>("demo-app",args: args);

var app = await builder.BuildAsync();

await app.RunAsync();