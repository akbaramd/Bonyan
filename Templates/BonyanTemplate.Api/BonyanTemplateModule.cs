using Bonyan.IdentityManagement.Web;
using Bonyan.Modularity;
using Bonyan.TenantManagement.Web;
using BonyanTemplate.Application;
using BonyanTemplate.Domain.Entities;
using BonyanTemplate.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace BonyanTemplate.Api;

public class BonyanTemplateModule : BonWebModule
{
    public BonyanTemplateModule()
    {
        DependOn<BonTenantManagementWebModule>();
        DependOn<BonIdentityManagementWebModule<User>>();
        DependOn<BonyanTemplateApplicationModule>();
        DependOn<BonaynTempalteInfrastructureModule>();
    }

    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
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