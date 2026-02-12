using Bonyan.IdentityManagement.Domain.Roles.DomainServices;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.IdentityManagement.Domain.Users.DomainServices;
using Bonyan.Modularity;
using Bonyan.UserManagement.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.IdentityManagement.Domain;

/// <summary>
/// Domain module for identity: users, roles, claims, tokens, and user meta (WordPress-style).
/// Uses final types <see cref="BonIdentityUser"/> and <see cref="BonIdentityRole"/> (non-generic).
/// Depends on <see cref="BonUserManagementDomainModule{TUser}"/> with <see cref="BonIdentityUser"/> for base user aggregate.
/// </summary>
public class BonIdentityManagementDomainModule : Modularity.Abstractions.BonModule
{
    public BonIdentityManagementDomainModule()
    {
        DependOn([typeof(BonUserManagementDomainModule<BonIdentityUser>)]);
    }

    public override ValueTask OnConfigureAsync(BonConfigurationContext context, CancellationToken cancellationToken = default)
    {
        context.Services.AddTransient<BonIdentityUserManager>();
        context.Services.AddTransient<IBonIdentityUserManager, BonIdentityUserManager>();
        context.Services.AddTransient<IBonIdentityRoleManager, BonIdentityRoleManager>();
        context.Services.AddTransient<BonIdentityRoleManager>();
        return base.OnConfigureAsync(context, cancellationToken);
    }
}
