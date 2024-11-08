using Bonyan.AutoMapper;
using Bonyan.Modularity;
using Bonyan.TenantManagement.Application;
using Bonyan.TenantManagement.Application.Profiles;
using Bonyan.TenantManagement.Application.Services;

namespace Bonyan.TenantManagement.Web;


public class BonTenantManagementWebModule : BonWebModule
{
    public BonTenantManagementWebModule()
    {
        DependOn([
            typeof(BonTenantManagementApplicationModule),
        ]);
    }
    public override Task OnPreConfigureAsync(BonConfigurationContext context)
    {
        context.Services.Configure<TenantManagementEndpointOptions>(c =>
        {
            c.BaseEndpoint = "/api/tenant-management";
        });
        return base.OnPreConfigureAsync(context);
    }

    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        context.Services.AddTransient<IBonTenantBonApplicationService, BonTenantBonApplicationService>();

        context.Services.Configure<BonAutoMapperOptions>(c => { c.AddProfile<BonTenantProfile>(true); });

        return base.OnConfigureAsync(context);
    }
}