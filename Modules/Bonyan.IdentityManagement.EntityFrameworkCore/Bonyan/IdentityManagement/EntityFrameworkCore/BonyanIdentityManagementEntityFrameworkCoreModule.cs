using Bonyan.EntityFrameworkCore;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.MultiTenant;
using Bonyan.UserManagement.Domain;
using Bonyan.UserManagement.Domain.Repositories;
using Bonyan.UserManagement.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.IdentityManagement.EntityFrameworkCore;

public class BonyanIdentityManagementEntityFrameworkCoreModule<TUser> : Module where TUser : BonyanUser
{
    public BonyanIdentityManagementEntityFrameworkCoreModule()
    {
        DependOn<BonyanUserManagementEntityFrameworkModule<TUser>>();
    }

    public override Task OnConfigureAsync(ServiceConfigurationContext context)
    {
        context.AddBonyanDbContext<BonIdentityManagementDbContext<TUser>>();
        return base.OnConfigureAsync(context);
    }
}