using Bonyan.AdminLte;
using Bonyan.AdminLte.Blazor.IdentityManagement;
using Bonyan.AspNetCore.Components;
using Bonyan.AspNetCore.Components.Menus;
using Bonyan.Modularity;
using Bonyan.TenantManagement.Web;
using Bonyan.Ui.Blazimum;
using BonyanTemplate.Application;
using BonyanTemplate.Blazor.Components;
using BonyanTemplate.Blazor.Menus;
using BonyanTemplate.Blazor.Themes;
using BonyanTemplate.Domain.Entities;
using BonyanTemplate.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Options;

namespace BonyanTemplate.Blazor;

public class BonyanTemplateBlazorModule : BonWebModule
{
    public BonyanTemplateBlazorModule()
    {
        DependOn<BonyanTemplateApplicationModule>();
        DependOn<BonaynTempalteInfrastructureModule>();
        DependOn<BonAspNetCoreComponentsModule>();
        DependOn<BonUiBlazimumModule>();
        DependOn<BonyanAdminLteBlazorIdentityManagementModule<User>>();
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

    public override Task OnApplicationAsync(BonContext context)
    {
        // Middleware
        context.Application.UseAuthentication();
        context.Application.UseAuthorization();

        return base.OnApplicationAsync(context);
    }

    public override Task OnPostApplicationAsync(BonContext context)
    {
        context.Application.UseStaticFiles();
        context.Application.UseAntiforgery();

        var otins = context.RequireService<IOptions<BonBlazorOptions>>();

        context.Application.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode()
            .AddAdditionalAssemblies(otins.Value.AdditionalAssembly.ToArray());

        return base.OnPostApplicationAsync(context);
    }
}