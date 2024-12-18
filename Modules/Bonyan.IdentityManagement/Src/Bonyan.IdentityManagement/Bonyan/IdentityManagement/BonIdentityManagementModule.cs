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
        DependOn<BonAuthenticationModule>();
        DependOn<BonIdentityManagementDomainModule<TUser>>();
    }

    public override Task OnPreConfigureAsync(BonConfigurationContext context)
    {
        context.Services.AddSingleton<IBonPermissionManager, BonPermissionManager>();

        
        
        PreConfigure<AuthorizationOptions>(c =>
        {
            var permissionAccessor = context.Services.GetRequiredService<IBonPermissionManager>();

            foreach (var permission in permissionAccessor.GetAllPermissions())
            {
                // Create a policy with the required permissions
                c.AddPolicy(permission.Id.Value, policy =>
                    policy.Requirements.Add(new BonPermissionRequirement(permission)));
            }

        });
        PreConfigure<AuthenticationBuilder>(c =>
        {
            var jwt = new BonAuthenticationJwtOptions();
            context.Services.ExecutePreConfiguredActions(jwt);
            context.Services.AddSingleton(jwt);
            if (jwt.Enabled && !string.IsNullOrWhiteSpace(jwt.SecretKey))
            {
                c.AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = jwt.RequireHttpsMetadata;
                    options.SaveToken = jwt.SaveToken;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = !string.IsNullOrEmpty(jwt.Issuer),
                        ValidateAudience = !string.IsNullOrEmpty(jwt.Audience),
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SecretKey)),
                        ValidIssuer = jwt.Issuer,
                        ValidAudience = jwt.Audience,
                        ClockSkew = TimeSpan.Zero // Optional: Adjust token expiration tolerance
                    };
                });

            }
            else
            {
                throw new InvalidOperationException("JWT Authentication requires a valid SecretKey and must be enabled.");
            }
        });

        return base.OnPreConfigureAsync(context);
    }


    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        
        
        context.Services.AddSingleton<IAuthorizationHandler, BonIdentityPermissionHandler>();
        // Register the ClaimProviderManager
        context.Services.AddTransient<IBonIdentityClaimProvider<TUser>, DefaultClaimProvider<TUser>>();
        context.Services.AddTransient<IBonIdentityClaimProviderManager<TUser>, ClaimProviderManager<TUser>>();
        
        return base.OnConfigureAsync(context);
    }

    public override Task OnPostConfigureAsync(BonConfigurationContext context)
    {
        context.Services.ExecutePreConfiguredActions(new BonIdentityManagementOptions(context));
        return base.OnPostConfigureAsync(context);
    }
}