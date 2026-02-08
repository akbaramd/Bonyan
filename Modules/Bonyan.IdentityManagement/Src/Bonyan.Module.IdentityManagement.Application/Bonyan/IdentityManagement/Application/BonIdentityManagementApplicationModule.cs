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

    public override ValueTask OnPreConfigureAsync(BonPreConfigurationContext context , CancellationToken cancellationToken = default)
    {
        return base.OnPreConfigureAsync(context);
    }

    public override ValueTask OnConfigureAsync(BonConfigurationContext context , CancellationToken cancellationToken = default)
    {
        
       
        
        return base.OnConfigureAsync(context);
    }

}