using Bonyan.AspNetCore.Components;
using Bonyan.AspNetCore.Components.Menus;
using Bonyan.Modularity;
using BonyanTemplate.Application;
using BonyanTemplate.Blazor.Components;
using BonyanTemplate.Blazor.Menus;
using BonyanTemplate.Blazor.Themes;
using BonyanTemplate.Domain.Entities;
using BonyanTemplate.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace BonyanTemplate.Blazor;

public class BonyanTemplateBlazorModule : BonWebModule
{
    public BonyanTemplateBlazorModule()
    {
        DependOn<BonyanTemplateApplicationModule>();
        DependOn<BonaynTempalteInfrastructureModule>();
        DependOn<BonAspNetCoreComponentsModule>();
    }

    public override Task OnPreConfigureAsync(BonConfigurationContext context)
    {
        Configure<BonBlazorOptions>(c => { c.AppAssembly = typeof(Program).Assembly; });

        context.Services.AddTransient<IMenuProvider, UserManagementMenuProvider>();
        context.Services.AddTransient<IMenuProvider, MainMenuProvider>();

        context.Services.AddSingleton<ThemeService>();
        return base.OnPreConfigureAsync(context);
    }

    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        // Identity options
        context.Services.Configure<IdentityOptions>(options =>
        {
            // Customize as needed
        });
        // Add services to the container.
        context.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        return base.OnConfigureAsync(context);
    }

    public override Task OnApplicationAsync(BonWebApplicationContext webApplicationContext)
    {
        // Middleware
        // webApplicationContext.Application.UseAuthentication();
        // webApplicationContext.Application.UseAuthorization();

        return base.OnApplicationAsync(webApplicationContext);
    }

    public override Task OnPostApplicationAsync(BonWebApplicationContext webApplicationContext)
    {
        webApplicationContext.Application.UseStaticFiles();
        webApplicationContext.Application.UseAntiforgery();

        var otins = webApplicationContext.RequireService<IOptions<BonBlazorOptions>>();

        webApplicationContext.Application.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode()
            .AddAdditionalAssemblies(otins.Value.AdditionalAssembly.ToArray());

        return base.OnPostApplicationAsync(webApplicationContext);
    }
}