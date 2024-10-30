using Bonyan.AutoMapper;
using Bonyan.Modularity;
using Bonyan.TenantManagement.Application;
using Bonyan.TenantManagement.Application.Profiles;
using Bonyan.TenantManagement.Application.Services;

namespace Bonyan.TenantManagement.Web;

[DependOn([
    typeof(BonyanTenantManagementApplicationModule),
])]
public class BonyanTenantManagementWebModule : WebModule
{
    public override Task OnPreConfigureAsync(ServiceConfigurationContext context)
    {
        context.Services.Configure<TenantManagementEndpointOptions>(c =>
        {
            c.BaseEndpoint = "/api/tenant-management";
        });
        return base.OnPreConfigureAsync(context);
    }

    public override Task OnConfigureAsync(ServiceConfigurationContext context)
    {
        context.Services.AddTransient<ITenantApplicationService, TenantApplicationService>();

        context.Services.Configure<BonyanAutoMapperOptions>(c => { c.AddProfile<TenantProfile>(true); });

        return base.OnConfigureAsync(context);
    }
}