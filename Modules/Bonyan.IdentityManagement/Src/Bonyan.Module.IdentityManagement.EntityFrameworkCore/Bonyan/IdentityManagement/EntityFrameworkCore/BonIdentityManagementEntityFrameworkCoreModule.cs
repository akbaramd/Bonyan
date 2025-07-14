using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Roles.Repostories;
using Bonyan.IdentityManagement.Domain.Roles.DomainServices;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.IdentityManagement.Domain.Users.Repositories;
using Bonyan.IdentityManagement.EntityFrameworkCore.Repositories;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.UserManagement.Domain.Users;
using Bonyan.UserManagement.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.IdentityManagement.EntityFrameworkCore;

public class BonIdentityManagementEntityFrameworkCoreModule<TUser, TRole> : BonModule 
    where TUser : BonIdentityUser<TUser, TRole> 
    where TRole :  BonIdentityRole<TRole>
{
    public BonIdentityManagementEntityFrameworkCoreModule()
    {
        DependOn<BonUserManagementEntityFrameworkModule<TUser>>();
    }

    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
       

        context.Services
            .AddTransient<IBonIdentityRoleRepository<TRole>, BonIdentityEfCoreRoleRepository<TUser, TRole>>();
        context.Services
            .AddTransient<IBonIdentityRoleReadOnlyRepository<TRole>, BonIdentityEfCoreRoleRepository<TUser, TRole>>();

        context.Services
            .AddTransient<IBonIdentityUserClaimsRepository<TUser, TRole>, BonEfCoreIdentityUserClaimsRepository<TUser, TRole>>();
        context.Services
            .AddTransient<IBonIdentityUserClaimsReadOnlyRepository<TUser, TRole>, BonEfCoreIdentityUserClaimsReadOnlyRepository<TUser, TRole>>();

        context.Services
            .AddTransient<IBonIdentityRoleClaimsRepository<TRole>, BonEfCoreIdentityRoleClaimsRepository<TUser, TRole>>();
        context.Services
            .AddTransient<IBonIdentityRoleClaimsReadOnlyRepository<TRole>, BonEfCoreIdentityRoleClaimsReadOnlyRepository<TUser, TRole>>();
            

        context.Services
            .AddTransient<IBonIdentityUserRepository<TUser, TRole>, BonEfCoreIdentityUserRepository<TUser, TRole>>();
        context.Services
            .AddTransient<IBonIdentityUserReadOnlyRepository<TUser, TRole>, BonEfCoreIdentityUserReadOnlyRepository<TUser, TRole>>();

        context.Services
            .AddTransient<IBonIdentityUserRolesRepository<TUser, TRole>, BonEfCoreIdentityUserRolesRepository<TUser, TRole>>();

        // Register generic role manager
        context.Services
            .AddTransient(typeof(IBonIdentityRoleManager<>), typeof(BonIdentityRoleManager<>));

        return base.OnConfigureAsync(context);
    }
}