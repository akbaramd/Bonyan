using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Roles.DomainServices;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.IdentityManagement.Domain.Users.DomainServices;
using Bonyan.Modularity;
using Bonyan.UserManagement.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.IdentityManagement.Domain;

public class BonIdentityManagementDomainModule<TUser,TRole> : Modularity.Abstractions.BonModule 
    where TUser : BonIdentityUser<TUser,TRole> 
    where TRole : BonIdentityRole<TRole>
{
    public BonIdentityManagementDomainModule()
    {
        DependOn([
            typeof(BonUserManagementDomainModule<TUser>),
        ]);
    }

    public override ValueTask OnConfigureAsync(BonConfigurationContext context , CancellationToken cancellationToken = default)
    {
        context.Services.AddTransient<BonIdentityUserManager<TUser,TRole>>();
        context.Services.AddTransient<IBonIdentityRoleManager<TRole>, BonIdentityRoleManager<TRole>>();
        context.Services.AddTransient<IBonIdentityUserManager<TUser,TRole>, BonIdentityUserManager<TUser,TRole>>();
        

   
        return base.OnConfigureAsync(context);
    }
}