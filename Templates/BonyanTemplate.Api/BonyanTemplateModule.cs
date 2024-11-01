using Bonyan.IdentityManagement.Web;
using Bonyan.Modularity;
using Bonyan.TenantManagement.Web;
using BonyanTemplate.Application;
using BonyanTemplate.Domain.Entities;
using BonyanTemplate.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace BonyanTemplate.Api;

public class BonyanTemplateModule : WebModule
{
    public BonyanTemplateModule()
    {
        DependOn<BonyanTenantManagementWebModule>();
        DependOn<BonyanIdentityManagementWebModule<User>>();
        DependOn<BonyanTemplateApplicationModule>();
        DependOn<BonaynTempalteInfrastructureModule>();
    }

    public override Task OnConfigureAsync(ServiceConfigurationContext context)
    {
        context.Services.Configure<IdentityOptions>(c =>
        {

        });
        return base.OnConfigureAsync(context);
    }

    public override Task OnPreApplicationAsync(ApplicationContext context)
    {
        // context.Application.UseUnitOfWork();
        return base.OnPreApplicationAsync(context);
    }

    public override Task OnApplicationAsync(ApplicationContext context)
    {
        context.Application.MapTenantManagementEndpoints();
        return base.OnApplicationAsync(context);
    }
}