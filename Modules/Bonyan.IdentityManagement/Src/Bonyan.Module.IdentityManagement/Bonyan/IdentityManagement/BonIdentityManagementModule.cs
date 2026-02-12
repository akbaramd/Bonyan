using Bonyan.AspNetCore.Authentication;
using Bonyan.IdentityManagement.ClaimProvider;
using Bonyan.IdentityManagement.Domain;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.IdentityManagement.Options;
using Bonyan.IdentityManagement.Permissions;
using Bonyan.Localization;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.IdentityManagement;

/// <summary>
/// Core identity management module. Configure via <see cref="BonIdentityManagementOptions"/> in PostConfigure.
/// Other modules can add claims by implementing <see cref="IBonIdentityClaimProvider"/> and registering in DI;
/// they receive <see cref="BonIdentityClaimProviderContext"/> (no domain entity). Permissions: implement
/// <see cref="IBonPermissionDefinitionProvider"/> and register in DI.
/// </summary>
public class BonIdentityManagementModule : BonModule
{
    public BonIdentityManagementModule()
    {
        DependOn<BonAspnetCoreAuthenticationModule>();
        DependOn<BonIdentityManagementDomainModule>();
        DependOn<BonLocalizationModule>();
    }

    public override ValueTask OnConfigureAsync(BonConfigurationContext context, CancellationToken cancellationToken = default)
    {
        // IdentityManagement localization is registered by BonIdentityManagementBonWebMvcModule with
        // type IdentityManagementResource and path /Localization/Resources/IdentityManagement so that
        // IStringLocalizer<IdentityManagementResource> in views resolves to the Bon localizer and JSON.

        context.Services.AddSingleton<IBonPermissionManager, BonPermissionManager>();
        context.Services.AddTransient<IBonPermissionDefinitionProvider, BonIdentityManagementPermissionDefinitionProvider>();
        context.Services.AddSingleton<IAuthorizationPolicyProvider, BonPermissionPolicyProvider>();

        context.Services.PreConfigure<AuthorizationOptions>(c =>
        {
            var permissionAccessor = context.Services.GetRequiredService<IBonPermissionManager>();
            foreach (var permission in permissionAccessor.GetAllPermissions())
                c.AddPolicy(permission.Name, policy =>
                    policy.Requirements.Add(new BonPermissionRequirement(permission.Name)));
        });

        context.Services.AddSingleton<IAuthorizationHandler, BonIdentityPermissionHandler>();

        context.Services.AddTransient<IBonIdentityClaimProvider, DefaultClaimProvider>();
        context.Services.AddTransient<IBonIdentityClaimProviderManager, ClaimProviderManager>();

        context.Services.AddSingleton<IUserTokenHasher, Sha256UserTokenHasher>();

        return base.OnConfigureAsync(context, cancellationToken);
    }

    public override ValueTask OnPostConfigureAsync(BonPostConfigurationContext context, CancellationToken cancellationToken = default)
    {
        context.Services.ExecutePreConfiguredActions(new BonIdentityManagementOptions(context));
        return base.OnPostConfigureAsync(context, cancellationToken);
    }
}
