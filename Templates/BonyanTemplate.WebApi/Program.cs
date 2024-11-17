using BonyanTemplate.WebApi;

var application = BonyanApplication.CreateModularApplication<BonyanTemplateWebApiModule>("BonyanTemplate",args);

await application.RunAsync();

