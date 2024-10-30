using Bonyan.Job.Hangfire;
using Bonyan.Modularity;
using Bonyan.TenantManagement.Web;
using BonyanTemplate.Application;
using BonyanTemplate.Infrastructure;

namespace BonyanTemplate.Api;

[DependOn(
    // typeof(BonyanJobHangfireModule),
    typeof(BonyanTenantManagementWebModule),
    typeof(BonyanTemplateApplicationModule),
    typeof(BonyanJobHangfireModule),
    typeof(InfrastructureModule)
)]
public class BonyanTemplateModule : WebModule
{
    public override Task OnApplicationAsync(ApplicationContext context)
    {
        context.Application.MapTenantManagementEndpoints();
        return base.OnApplicationAsync(context);
    }
}