using System.Text;
using Bonyan.AspNetCore.Authentication;
using Bonyan.IdentityManagement.Domain;
using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.IdentityManagement.Options;
using Bonyan.IdentityManagement.Permissions;
using Bonyan.Localization;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;


namespace Bonyan.IdentityManagement;

public class BonIdentityManagementModule<TUser,TRole> : BonModule where TUser : BonIdentityUser<TUser,TRole> where TRole : BonIdentityRole<TRole>
{
    public BonIdentityManagementModule()
    {
        DependOn<BonAspnetCoreAuthenticationModule>();
        DependOn<BonIdentityManagementDomainModule<TUser,TRole>>();
        DependOn<BonLocalizationModule>();
    }

    public override Task OnPreConfigureAsync(BonConfigurationContext context)
    {
        // Register localization resource for this module
        PreConfigure<BonLocalizationOptions>(options =>
        {
            options.Resources
                .Add<BonIdentityManagementResource>("en")
                .AddVirtualJson("/Localization/IdentityManagement");
        });

        context.Services.AddSingleton<IBonPermissionManager<TUser,TRole>, BonPermissionManager<TUser,TRole>>();

        // Register permission definition providers
        context.Services.AddTransient<IBonPermissionDefinitionProvider, BonIdentityManagementPermissionDefinitionProvider>();
        
        // Register dynamic policy provider
        context.Services.AddSingleton<IAuthorizationPolicyProvider, BonPermissionPolicyProvider<TUser,TRole>>();
        
        PreConfigure<AuthorizationOptions>(c =>
        {
            var permissionAccessor = context.Services.GetRequiredService<IBonPermissionManager<TUser,TRole>>();

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
        context.Services.AddSingleton<IAuthorizationHandler, BonIdentityPermissionHandler<TUser,TRole>>();
        
        // Register the ClaimProviderManager
        context.Services.AddTransient<IBonIdentityClaimProvider<TUser,TRole>, DefaultClaimProvider<TUser,TRole>>();
        context.Services.AddTransient<IBonIdentityClaimProviderManager<TUser,TRole>, ClaimProviderManager<TUser,TRole>>();
        
        return base.OnConfigureAsync(context);
    }

    public override Task OnPostConfigureAsync(BonConfigurationContext context)
    {
        // Execute pre-configured actions for identity management options
        context.Services.ExecutePreConfiguredActions(new BonIdentityManagementOptions(context));
        
        return base.OnPostConfigureAsync(context);
    }
}

// Marker resource class for localization
[LocalizationResourceName("IdentityManagement")]
public class BonIdentityManagementResource { }