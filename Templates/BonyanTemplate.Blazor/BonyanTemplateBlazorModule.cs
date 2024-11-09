using Bonyan.AspNetCore.Components;
using Bonyan.AspNetCore.Components.Menus;
using Bonyan.IdentityManagement.Web;
using Bonyan.Modularity;
using Bonyan.TenantManagement.Web;
using BonyanTemplate.Application;
using BonyanTemplate.Blazor.Components;
using BonyanTemplate.Blazor.Menus;
using BonyanTemplate.Blazor.Middlewares;
using BonyanTemplate.Blazor.Themes;
using BonyanTemplate.Domain.Entities;
using BonyanTemplate.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace BonyanTemplate.Blazor;

public class BonyanTemplateBlazorModule : BonWebModule
{
    public BonyanTemplateBlazorModule()
    {
        DependOn<BonTenantManagementWebModule>();
        DependOn<BonIdentityManagementWebModule<User>>();
        DependOn<BonyanTemplateApplicationModule>();
        DependOn<BonaynTempalteInfrastructureModule>();
        DependOn<BonAspNetCoreComponentsModule>();
    }

    public override Task OnPreConfigureAsync(BonConfigurationContext context)
    {
        context.Services.AddTransient<IMenuProvider, UserManagementMenuProvider>();
        context.Services.AddTransient<IMenuProvider, MainMenuProvider>();

        context.Services.AddSingleton<ThemeService>();
        return base.OnPreConfigureAsync(context);
    }

    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        Configure<BonBlazorOptions>(c =>
        {
            c.AppAssembly = typeof(Program).Assembly;
            c.AdditionalAssembly = AppDomain.CurrentDomain.GetAssemblies();
        });

        context.Services.Configure<IdentityOptions>(c => { });
        
        // Add services to the container.
        context.Services.AddRazorComponents()
            .AddInteractiveServerComponents();
        
        return base.OnConfigureAsync(context);
    }


    public override Task OnPostInitializeAsync(ServiceInitializationContext context)
    {
        return base.OnPostInitializeAsync(context);
    }

    public override Task OnPreInitializeAsync(ServiceInitializationContext context)
    {
        return base.OnPreInitializeAsync(context);
    }

    public override Task OnPreApplicationAsync(BonContext context)
    {
        // Configure the HTTP request pipeline.
        if (!context.Application.Environment.IsDevelopment())
        {
            context.Application.UseExceptionHandler("/Error", createScopeForErrors: true);
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            context.Application.UseHsts();
        }
        
    
        
        return base.OnPreApplicationAsync(context);
    }

    public override Task OnApplicationAsync(BonContext context)
    {
        context.Application.UseMiddleware<BonAdminLteAssetInjectionMiddleware>();
        context.Application.MapTenantManagementEndpoints();
        return base.OnApplicationAsync(context);
    }

    
    public override Task OnPostApplicationAsync(BonContext context)
    {
        
        context.Application.UseAuthentication();
        context.Application.UseAuthorization();
        context.Application.UseStaticFiles();
        context.Application.UseAntiforgery();

        context.Application.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

      
        return base.OnPostApplicationAsync(context);
    }
}