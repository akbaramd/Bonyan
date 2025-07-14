using Bonyan.AutoMapper;
using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.UserManagement.Application;
using Bonyan.Workers;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.IdentityManagement.Application;

public class BonIdentityManagementApplicationModule<TUser,TRole> : Modularity.Abstractions.BonModule where TUser : BonIdentityUser<TUser,TRole> where TRole : BonIdentityRole<TRole>
{
    public BonIdentityManagementApplicationModule()
    {
        DependOn<BonUserManagementApplicationModule<TUser>>();
        DependOn<BonIdentityManagementModule<TUser,TRole>>();
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