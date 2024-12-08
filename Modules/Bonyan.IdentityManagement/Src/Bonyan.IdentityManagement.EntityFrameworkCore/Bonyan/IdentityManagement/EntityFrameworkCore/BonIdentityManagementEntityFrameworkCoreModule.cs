using Bonyan.IdentityManagement.Domain.Permissions.Repositories;
using Bonyan.IdentityManagement.Domain.Roles.Repostories;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.IdentityManagement.Domain.Users.Repositories;
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
       

        context.Services
            .AddTransient<IBonIdentityRoleRepository, BonIdentityEfCoreRoleRepository<TUser>>();
        context.Services
            .AddTransient<IBonIdentityRoleReadOnlyRepository, BonIdentityEfCoreRoleRepository<TUser>>();

        context.Services
            .AddTransient<IBonIdentityPermissionRepository, BonIdentityEfCorePermissionRepository<TUser>>();
        context.Services
            .AddTransient<IBonIdentityPermissionReadOnlyRepository, BonIdentityEfCorePermissionReadonlyRepository<TUser>>();
        

        context.Services
            .AddTransient<IBonIdentityUserRepository<TUser>, BonEfCoreIdentityUserRepository<TUser>>();
        

        context.Services
            .AddTransient<IBonIdentityUserRolesRepository, BonEfCoreIdentityUserRolesRepository<TUser>>();
        context.Services
            .AddTransient<IBonIdentityRolePermissionRepository, BonEfCoreIdentityRolePermissionsRepository<TUser>>();
        return base.OnConfigureAsync(context);
    }
}