using Bonyan.AspNetCore.Components;
using BonyanTemplate.Blazor;
using BonyanTemplate.Blazor.Components;
using BonyanTemplate.Blazor.Themes;
using Microsoft.AspNetCore.Authentication.Cookies;

var application = BonyanApplication.CreateModularApplication<BonyanTemplateBlazorModule>(args);

await application.RunAsync();
