using Bonyan.IdentityManagement.Web;
using Bonyan.Job.Hangfire;
using Bonyan.Modularity;
using Bonyan.TenantManagement.Web;
using BonyanTemplate.Application;
using BonyanTemplate.Domain.Entities;
using BonyanTemplate.Infrastructure;

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

    public override Task OnApplicationAsync(ApplicationContext context)
    {
        context.Application.MapTenantManagementEndpoints();
        return base.OnApplicationAsync(context);
    }
}