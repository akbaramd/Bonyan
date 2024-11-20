using Bonyan.AspNetCore.Authentication;
using Bonyan.IdentityManagement.Domain;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.IdentityManagement;

public class BonIdentityManagementModule<TUser> : BonModule where TUser : class, IBonIdentityUser
{
    public BonIdentityManagementModule()
    {
        DependOn<BonAuthenticationModule>();
        DependOn<BonIdentityManagementDomainModule<TUser>>();
    }

    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        // Register the ClaimProviderManager
        context.Services.AddTransient<IBonIdentityClaimProvider, DefaultClaimProvider>();
        context.Services.AddTransient<IBonIdentityClaimProviderManager, ClaimProviderManager>();
        
        return base.OnConfigureAsync(context);
    }
}