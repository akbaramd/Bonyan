using Bonyan.AutoMapper;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.UserManagement.Application;
using Bonyan.Workers;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.IdentityManagement.Application;

public class BonIdentityManagementApplicationModule : BonModule 
{
    public BonIdentityManagementApplicationModule()
    {
        DependOn<BonUserManagementApplicationModule<BonIdentityUser>>();
        DependOn<BonIdentityManagementModule<BonIdentityUser>>();
        DependOn<BonWorkersModule>();
    }

    public override Task OnPreConfigureAsync(BonConfigurationContext context)
    {
        return base.OnPreConfigureAsync(context);
    }

    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        
       
        
        return base.OnConfigureAsync(context);
    }

    public override async Task OnPostInitializeAsync(BonInitializedContext context)
    {
        await base.OnPostInitializeAsync(context);
    }
}