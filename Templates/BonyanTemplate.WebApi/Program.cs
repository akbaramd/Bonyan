using Bonyan.Plugins;
using BonyanTemplate.WebApi;

var builder = BonyanApplication.CreateModularBuilder<BonyanTemplateWebApiModule>("BonyanTemplate", "Bonyan Template Web API", v =>
{
});
var app = await builder.BuildAsync();
await app.RunAsync();