using BonyanTemplate.Blazor;

var application = BonyanApplication.CreateModularApplication<BonyanTemplateBlazorModule>("BonyanTemplate",args);

await application.RunAsync();

