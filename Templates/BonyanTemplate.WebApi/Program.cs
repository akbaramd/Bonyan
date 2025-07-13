using Bonyan.Plugins;
using BonyanTemplate.WebApi;

var builder = BonyanApplication.CreateModularBuilder<BonyanTemplateWebApiModule>("BonyanTemplate", v =>
{
});
var app = await builder.BuildAsync();
await app.RunAsync();