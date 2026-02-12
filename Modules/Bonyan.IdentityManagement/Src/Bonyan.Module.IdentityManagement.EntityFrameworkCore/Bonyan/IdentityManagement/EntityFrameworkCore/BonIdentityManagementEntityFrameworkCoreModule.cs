using Bonyan.EntityFrameworkCore;
using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Roles.Repositories;
using Bonyan.IdentityManagement.Domain.Roles.ValueObjects;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.IdentityManagement.Domain.Users.Repositories;
using Bonyan.IdentityManagement.Domain.Users.UserMeta;
using Bonyan.IdentityManagement.EntityFrameworkCore.Repositories;
using Bonyan.Layer.Domain.Repository.Abstractions;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.UserManagement.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.IdentityManagement.EntityFrameworkCore;

/// <summary>
/// EF Core module for identity management (non-generic). Registers repositories for
/// <see cref="BonIdentityUser"/>, <see cref="BonIdentityRole"/>, and user meta.
/// </summary>
public class BonIdentityManagementEntityFrameworkCoreModule : BonModule
{
    public BonIdentityManagementEntityFrameworkCoreModule()
    {
        DependOn<BonEntityFrameworkModule>();
        DependOn<BonUserManagementEntityFrameworkModule<Bonyan.IdentityManagement.Domain.Users.BonIdentityUser>>();
    }

    public override ValueTask OnConfigureAsync(BonConfigurationContext context, CancellationToken cancellationToken = default)
    {
        context.Services.AddTransient<IBonIdentityRoleRepository, BonIdentityEfCoreRoleRepository>();
        context.Services.AddTransient<IBonIdentityRoleReadOnlyRepository, BonIdentityEfCoreRoleRepository>();
        context.Services.AddTransient<IBonReadOnlyRepository<BonIdentityRole, BonRoleId>>(sp => sp.GetRequiredService<IBonIdentityRoleReadOnlyRepository>());

        context.Services.AddTransient<IBonIdentityUserClaimsRepository, BonEfCoreIdentityUserClaimsRepository>();
        context.Services.AddTransient<IBonIdentityUserClaimsReadOnlyRepository, BonEfCoreIdentityUserClaimsReadOnlyRepository>();

        context.Services.AddTransient<IBonIdentityRoleClaimsRepository, BonEfCoreIdentityRoleClaimsRepository>();
        context.Services.AddTransient<IBonIdentityRoleClaimsReadOnlyRepository, BonEfCoreIdentityRoleClaimsReadOnlyRepository>();

        context.Services.AddTransient<IBonIdentityUserRepository, BonEfCoreIdentityUserRepository>();
        context.Services.AddTransient<IBonIdentityUserReadOnlyRepository, BonEfCoreIdentityUserReadOnlyRepository>();

        context.Services.AddTransient<IBonIdentityUserRolesRepository, BonEfCoreIdentityUserRolesRepository>();

        context.Services.AddTransient<IBonUserMetaRepository, BonEfCoreUserMetaRepository>();

        return base.OnConfigureAsync(context, cancellationToken);
    }
}
