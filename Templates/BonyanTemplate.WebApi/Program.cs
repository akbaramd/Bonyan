using BonyanTemplate.WebApi;

var builder = BonyanApplication.CreateModularBuilder<BonyanTemplateWebApiModule>(c =>
{
    c.ApplicationName = "BonyanTemplate";
});
var app = await builder.BuildAsync();
await app.RunAsync();