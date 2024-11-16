using Bonyan.DependencyInjection;
using Bonyan.IdentityManagement.Domain;
using Bonyan.IdentityManagement.Domain.Abstractions.Permissions;
using Bonyan.IdentityManagement.Domain.Abstractions.Roles;
using Bonyan.IdentityManagement.Domain.Abstractions.Users;
using Bonyan.IdentityManagement.Domain.Permissions;
using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.IdentityManagement.EntityFrameworkCore.Repositories;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.UserManagement.Domain.Users.Repositories;
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

        context.Services.AddTransient<IBonIdentityRoleRepository, BonIdentityEfCoreRoleRepository>();
        context.Services.AddTransient<IBonIdentityRoleReadOnlyRepository, BonIdentityEfCoreRoleRepository>();
        context.Services.AddTransient<IBonIdentityPermissionRepository, BonIdentityEfCorePermissionRepository>();
        context.Services
            .AddTransient<IBonIdentityPermissionReadOnlyRepository, BonIdentityEfCorePermissionRepository>();

        context.Services.AddTransient<IBonIdentityUserRepository<TUser>, BonEfCoreIdentityUserRepository<TUser>>();
        context.Services
            .AddTransient<IBonIdentityUserReadOnlyRepository<TUser>, BonEfCoreIdentityUserRepository<TUser>>();

        context.Services.AddTransient<IBonIdentityUserRepository, BonEfCoreIdentityUserRepository>();
        context.Services.AddTransient<IBonIdentityUserReadOnlyRepository, BonEfCoreIdentityUserRepository>();

        return base.OnConfigureAsync(context);
    }
}