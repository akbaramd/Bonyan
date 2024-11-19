using BonyanTemplate.WebApi;

var builder = BonyanApplication.CreateModularBuilder<BonyanTemplateWebApiModule>("BonyanTemplate");
var app = await builder.BuildAsync();
await app.RunAsync();