using Bonyan.DependencyInjection;
using Bonyan.EntityFrameworkCore;
using Bonyan.IdentityManagement.Domain;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.UserManagement.Domain;
using Bonyan.UserManagement.EntityFrameworkCore;

namespace Bonyan.IdentityManagement.EntityFrameworkCore;

public class BonIdentityManagementEntityFrameworkCoreModule<TUser, TRole> : BonModule
    where TUser : BonUser where TRole : BonRole
{
    public BonIdentityManagementEntityFrameworkCoreModule()
    {
        DependOn<BonUserManagementEntityFrameworkModule<TUser>>();
    }

    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        context.AddBonDbContext<BonIdentityManagementDbContext<TUser, TRole>>();
        return base.OnConfigureAsync(context);
    }
}