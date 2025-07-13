using System.Text;
using Bonyan.AspNetCore.Authentication;
using Bonyan.IdentityManagement.Domain;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.IdentityManagement.Options;
using Bonyan.IdentityManagement.Permissions;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Bonyan.IdentityManagement;

public class BonIdentityManagementModule<TUser> : BonModule where TUser : BonIdentityUser
{
    public BonIdentityManagementModule()
    {
        DependOn<BonAspnetCoreAuthenticationModule>();
        DependOn<BonIdentityManagementDomainModule<TUser>>();
    }

    public override Task OnPreConfigureAsync(BonConfigurationContext context)
    {
        context.Services.AddSingleton<IBonPermissionManager, BonPermissionManager>();

        // Register permission definition providers
        context.Services.AddTransient<IBonPermissionDefinitionProvider, BonIdentityManagementPermissionDefinitionProvider>();
        
        // Register dynamic policy provider
        context.Services.AddSingleton<IAuthorizationPolicyProvider, BonPermissionPolicyProvider>();
        
        PreConfigure<AuthorizationOptions>(c =>
        {
            var permissionAccessor = context.Services.GetRequiredService<IBonPermissionManager>();

            foreach (var permission in permissionAccessor.GetAllPermissions())
            {
                // Create a policy with the required permissions
                c.AddPolicy(permission.Name, policy =>
                    policy.Requirements.Add(new BonPermissionRequirement(permission.Name)));
            }
        });

        return base.OnPreConfigureAsync(context);
    }

    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        // Register permission-based authorization handler
        context.Services.AddSingleton<IAuthorizationHandler, BonIdentityPermissionHandler>();
        
        // Register the ClaimProviderManager
        context.Services.AddTransient<IBonIdentityClaimProvider<TUser>, DefaultClaimProvider<TUser>>();
        context.Services.AddTransient<IBonIdentityClaimProviderManager<TUser>, ClaimProviderManager<TUser>>();
        
        return base.OnConfigureAsync(context);
    }

    public override Task OnPostConfigureAsync(BonConfigurationContext context)
    {
        // Execute pre-configured actions for identity management options
        context.Services.ExecutePreConfiguredActions(new BonIdentityManagementOptions(context));
        
        return base.OnPostConfigureAsync(context);
    }
}