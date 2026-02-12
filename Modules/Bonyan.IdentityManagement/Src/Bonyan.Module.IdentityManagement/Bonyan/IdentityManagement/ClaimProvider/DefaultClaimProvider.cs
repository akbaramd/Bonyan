using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace Bonyan.IdentityManagement.ClaimProvider;

/// <summary>
/// Default claim provider: adds standard identity claims from the context
/// (name, email, phone, profile, status). Other modules can register their own
/// <see cref="IBonIdentityClaimProvider"/> to add more claims.
/// </summary>
public class DefaultClaimProvider : IBonIdentityClaimProvider
{
    private readonly ILogger<DefaultClaimProvider> _logger;

    public DefaultClaimProvider(ILogger<DefaultClaimProvider> logger)
    {
        _logger = logger;
    }

    public Task<IEnumerable<Claim>> GenerateClaimsAsync(BonIdentityClaimProviderContext context)
    {
        var claims = new List<Claim>();

        claims.Add(new Claim(ClaimTypes.NameIdentifier, context.UserId));
        claims.Add(new Claim(ClaimTypes.Name, context.UserName ?? string.Empty));
        if (!string.IsNullOrEmpty(context.Email))
            claims.Add(new Claim(ClaimTypes.Email, context.Email));
        claims.Add(new Claim(ClaimTypes.GivenName, context.FirstName ?? string.Empty));
        claims.Add(new Claim(ClaimTypes.Surname, context.LastName ?? string.Empty));
        if (!string.IsNullOrEmpty(context.PhoneNumber))
            claims.Add(new Claim(ClaimTypes.MobilePhone, context.PhoneNumber));

        claims.Add(new Claim("email_verified", context.EmailVerified.ToString().ToLowerInvariant()));
        claims.Add(new Claim("phone_verified", context.PhoneVerified.ToString().ToLowerInvariant()));
        claims.Add(new Claim("created_at", context.CreatedAt));
        if (!string.IsNullOrEmpty(context.Status))
            claims.Add(new Claim("status", context.Status.ToLowerInvariant()));

        foreach (var roleName in context.RoleNames ?? Array.Empty<string>())
            claims.Add(new Claim(ClaimTypes.Role, roleName));

        _logger.LogDebug("DefaultClaimProvider generated {Count} claims for user {UserId}.", claims.Count, context.UserId);
        return Task.FromResult<IEnumerable<Claim>>(claims);
    }
}
