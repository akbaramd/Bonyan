using Bonyan.AspNetCore.Components;
using Bonyan.AspNetCore.Components.Admin;
using Bonyan.IdentityManagement.Web;
using Bonyan.Modularity;
using Bonyan.TenantManagement.Web;
using BonyanTemplate.Application;
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
        
        context.Services.Configure<IdentityOptions>(c =>
        {

        });
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
        // context.Application.UseUnitOfWork();
        return base.OnPreApplicationAsync(context);
    }

    public override Task OnApplicationAsync(BonContext context)
    {
        context.Application.MapTenantManagementEndpoints();
        return base.OnApplicationAsync(context);
    }
}