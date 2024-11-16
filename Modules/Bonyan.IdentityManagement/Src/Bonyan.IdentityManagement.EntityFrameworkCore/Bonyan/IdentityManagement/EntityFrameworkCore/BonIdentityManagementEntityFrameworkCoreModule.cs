using Bonyan.DependencyInjection;
using Bonyan.IdentityManagement.Domain;
using Bonyan.IdentityManagement.Domain.Abstractions.Permissions;
using Bonyan.IdentityManagement.Domain.Abstractions.Roles;
using Bonyan.IdentityManagement.Domain.Permissions;
using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.IdentityManagement.EntityFrameworkCore.Repositories;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.UserManagement.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.IdentityManagement.EntityFrameworkCore;

public class BonIdentityManagementEntityFrameworkCoreModule<TUser> : BonModule
    where TUser : class, IBonIdentityUser
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
        context.Services.AddTransient<IBonIdentityPermissionRepository, BonIdentityEfCorePermissionRepository>();
        return base.OnConfigureAsync(context);
    }
}