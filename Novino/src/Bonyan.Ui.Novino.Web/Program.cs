

using Bonyan.Novino.Web;
using Bonyan.Plugins;

var builder = BonyanApplication.CreateModularBuilder<BonNovinoWebModule>("Novino.Web", "Novino Web", v =>
{
    v.PlugInSources.AddFolder("./Plugins");
});
var app = await builder.BuildAsync();
await app.RunAsync();