using Bonyan.DependencyInjection;
using Bonyan.EntityFrameworkCore;
using Bonyan.IdentityManagement.Domain;
using Bonyan.IdentityManagement.EntityFrameworkCore.Repositories;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.UserManagement.Domain;
using Bonyan.UserManagement.Domain.Repositories;
using Bonyan.UserManagement.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.IdentityManagement.EntityFrameworkCore;

public class BonIdentityManagementEntityFrameworkCoreModule<TUser> : BonModule
    where TUser : BonIdentityUser
{
    public BonIdentityManagementEntityFrameworkCoreModule()
    {
        DependOn<BonUserManagementEntityFrameworkModule<TUser>>();
    }

    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        context.AddBonDbContext<BonIdentityManagementDbContext<TUser>>(c => { c.AddDefaultRepositories(true); });
        context.AddBonDbContext<BonIdentityManagementDbContext>(c => { c.AddDefaultRepositories(true); });

        context.Services.AddTransient<IBonRoleRepository, BonEfCoreRoleRepository>();
        context.Services.AddTransient<IBonRoleReadOnlyRepository, BonEfCoreRoleRepository>();
        context.Services.AddTransient<IBonPermissionRepository, BonEfCorePermissionRepository>();
        return base.OnConfigureAsync(context);
    }
}