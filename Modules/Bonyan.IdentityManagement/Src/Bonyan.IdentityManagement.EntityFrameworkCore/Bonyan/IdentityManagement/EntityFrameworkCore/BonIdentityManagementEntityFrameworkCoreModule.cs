using Bonyan.IdentityManagement.Domain.Permissions.Repositories;
using Bonyan.IdentityManagement.Domain.Roles.Repostories;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.IdentityManagement.Domain.Users.Repositories;
using Bonyan.IdentityManagement.EntityFrameworkCore.Repositories;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.UserManagement.Domain.Users;
using Bonyan.UserManagement.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.IdentityManagement.EntityFrameworkCore;

public class BonIdentityManagementEntityFrameworkCoreModule : BonModule
{
    public BonIdentityManagementEntityFrameworkCoreModule()
    {
        DependOn<BonUserManagementEntityFrameworkModule>();
    }

    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
       

        context.Services
            .AddTransient<IBonIdentityRoleRepository, BonIdentityEfCoreRoleRepository<BonIdentityUser>>();
        context.Services
            .AddTransient<IBonIdentityRoleReadOnlyRepository, BonIdentityEfCoreRoleRepository<BonIdentityUser>>();

        context.Services
            .AddTransient<IBonIdentityPermissionRepository, BonIdentityEfCorePermissionRepository<BonIdentityUser>>();
        context.Services
            .AddTransient<IBonIdentityPermissionReadOnlyRepository, BonIdentityEfCorePermissionReadonlyRepository<BonIdentityUser>>();
        

        context.Services
            .AddTransient<IBonIdentityUserRepository<BonIdentityUser>, BonEfCoreIdentityUserRepository<BonIdentityUser>>();
        

        context.Services
            .AddTransient<IBonIdentityUserRolesRepository, BonEfCoreIdentityUserRolesRepository<BonIdentityUser>>();
        context.Services
            .AddTransient<IBonIdentityRolePermissionRepository, BonEfCoreIdentityRolePermissionsRepository<BonIdentityUser>>();
        return base.OnConfigureAsync(context);
    }
}