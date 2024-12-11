using System.Security.Claims;
using Bonyan.IdentityManagement.Domain.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.IdentityManagement;

internal class ClaimProviderManager<TUser> : IBonIdentityClaimProviderManager<TUser> where TUser : BonIdentityUser
{
    private readonly IServiceProvider _serviceProvider;

    public ClaimProviderManager(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<IEnumerable<Claim>> GetClaimsAsync(TUser user)
    {
        // Resolve all claim providers from DI
        var claimProviders = _serviceProvider.GetServices<IBonIdentityClaimProvider<TUser>>();
        var claimsTasks = claimProviders.Select(provider => provider.GenerateClaimsAsync(user));
        var claimsArrays = await Task.WhenAll(claimsTasks);

        return claimsArrays.SelectMany(claims => claims);
    }

 
}

