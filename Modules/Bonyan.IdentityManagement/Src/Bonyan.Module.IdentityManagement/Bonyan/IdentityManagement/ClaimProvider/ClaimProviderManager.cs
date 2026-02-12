using System.Security.Claims;
using Bonyan.IdentityManagement.Domain;
using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.IdentityManagement.Domain.Roles.Repositories;
using Bonyan.IdentityManagement.Domain.Users;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Bonyan.IdentityManagement.ClaimProvider;

/// <summary>
/// Builds a context from the domain user (without exposing the entity to providers),
/// then aggregates claims from all registered <see cref="IBonIdentityClaimProvider"/> implementations.
/// </summary>
public class ClaimProviderManager : IBonIdentityClaimProviderManager
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ClaimProviderManager> _logger;
    private readonly IBonIdentityRoleReadOnlyRepository? _roleRepository;

    public ClaimProviderManager(
        IServiceProvider serviceProvider,
        ILogger<ClaimProviderManager> logger,
        IBonIdentityRoleReadOnlyRepository? roleRepository = null)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _roleRepository = roleRepository;
    }

    public async Task<IEnumerable<Claim>> GetClaimsAsync(BonIdentityUser user)
    {
        var context = await BuildContextAsync(user);
        var allClaims = new List<Claim>();

        var claimProviders = _serviceProvider.GetServices<IBonIdentityClaimProvider>();
        foreach (var provider in claimProviders)
        {
            try
            {
                var claims = await provider.GenerateClaimsAsync(context);
                allClaims.AddRange(claims);
                _logger.LogDebug("Provider {Provider} added {Count} claims for user {UserId}",
                    provider.GetType().Name, claims.Count(), user.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Provider {Provider} failed for user {UserId}; skipping.",
                    provider.GetType().Name, user.Id);
            }
        }

        return allClaims
            .GroupBy(c => new { c.Type, c.Value })
            .Select(g => g.First())
            .ToList();
    }

    private async Task<BonIdentityClaimProviderContext> BuildContextAsync(BonIdentityUser user)
    {
        var roleIds = user.UserRoles.Select(r => r.RoleId).ToList();
        var roleNames = new List<string>();
        if (roleIds.Count > 0 && _roleRepository != null)
        {
            var roles = await _roleRepository.FindAsync(r => roleIds.Contains(r.Id));
            roleNames = roles.Select(r => r.Title).ToList();
        }

        var permissionNames = user.GetClaimsByType(BonIdentityClaimTypes.Permission)
            .Select(c => c.ClaimValue)
            .ToList();

        var profile = user.Profile;

        return new BonIdentityClaimProviderContext
        {
            UserId = user.Id?.Value.ToString() ?? string.Empty,
            UserName = user.UserName ?? string.Empty,
            Email = user.Email?.Address,
            EmailVerified = user.Email?.IsVerified ?? false,
            PhoneNumber = user.PhoneNumber?.Number,
            PhoneVerified = user.PhoneNumber?.IsVerified ?? false,
            FirstName = profile?.FirstName,
            LastName = profile?.LastName,
            Status = user.Status?.Name?.ToString(),
            CreatedAt = user.CreatedAt.ToString("O"),
            RoleNames = roleNames,
            PermissionNames = permissionNames,
        };
    }
}
