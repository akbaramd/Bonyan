using Bonyan.EntityFrameworkCore;
using Bonyan.IdentityManagement.Domain;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.UserManagement.Domain;
using Bonyan.UserManagement.EntityFrameworkCore;

namespace Bonyan.IdentityManagement.EntityFrameworkCore;

public class BonyanIdentityManagementEntityFrameworkCoreModule<TUser, TRole> : Module
    where TUser : BonyanUser where TRole : BonRole
{
    public BonyanIdentityManagementEntityFrameworkCoreModule()
    {
        DependOn<BonyanUserManagementEntityFrameworkModule<TUser>>();
    }

    public override Task OnConfigureAsync(ServiceConfigurationContext context)
    {
        context.AddBonyanDbContext<BonIdentityManagementDbContext<TUser, TRole>>();
        return base.OnConfigureAsync(context);
    }
}