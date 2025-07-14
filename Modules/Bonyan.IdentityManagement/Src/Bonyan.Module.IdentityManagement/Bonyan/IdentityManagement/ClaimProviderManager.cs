using System.Security.Claims;
using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Users;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Bonyan.IdentityManagement
{
    /// <summary>
    /// Manages and aggregates claims from multiple claim providers
    /// </summary>
    public class ClaimProviderManager<TUser,TRole> : IBonIdentityClaimProviderManager<TUser,TRole> where TUser : BonIdentityUser<TUser,TRole> where TRole : BonIdentityRole<TRole>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ClaimProviderManager<TUser,TRole>> _logger;

        public ClaimProviderManager(
            IServiceProvider serviceProvider,
            ILogger<ClaimProviderManager<TUser,TRole>> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task<IEnumerable<Claim>> GetClaimsAsync(TUser user)
        {
            var allClaims = new List<Claim>();

            try
            {
                var claimProviders = _serviceProvider.GetServices<IBonIdentityClaimProvider<TUser,TRole>>();

                foreach (var provider in claimProviders)
                {
                    try
                    {
                        var claims = await provider.GenerateClaimsAsync(user);
                        allClaims.AddRange(claims);
                        
                        _logger.LogDebug("Claims provider {ProviderType} generated {ClaimCount} claims for user {UserId}", 
                            provider.GetType().Name, claims.Count(), user.Id);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error generating claims from provider {ProviderType} for user {UserId}", 
                            provider.GetType().Name, user.Id);
                        
                        // Continue with other providers even if one fails
                    }
                }

                // Remove duplicate claims (keeping the first occurrence)
                var uniqueClaims = allClaims
                    .GroupBy(c => new { c.Type, c.Value })
                    .Select(g => g.First())
                    .ToList();

                if (uniqueClaims.Count != allClaims.Count)
                {
                    _logger.LogDebug("Removed {DuplicateCount} duplicate claims for user {UserId}", 
                        allClaims.Count - uniqueClaims.Count, user.Id);
                }

                _logger.LogDebug("Generated total of {ClaimCount} unique claims for user {UserId}", 
                    uniqueClaims.Count, user.Id);

                return uniqueClaims;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error aggregating claims for user {UserId}", user.Id);
                throw;
            }
        }
    }
} 