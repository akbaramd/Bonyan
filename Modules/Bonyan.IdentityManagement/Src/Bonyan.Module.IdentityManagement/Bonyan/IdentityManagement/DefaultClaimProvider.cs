using System.Security.Claims;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.IdentityManagement.Permissions;
using Microsoft.Extensions.Logging;

namespace Bonyan.IdentityManagement
{
    /// <summary>
    /// Default implementation of claim provider that generates standard claims for users
    /// </summary>
    public class DefaultClaimProvider<TUser> : IBonIdentityClaimProvider<TUser> where TUser : BonIdentityUser
    {
        private readonly ILogger<DefaultClaimProvider<TUser>> _logger;

        public DefaultClaimProvider(ILogger<DefaultClaimProvider<TUser>> logger)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<Claim>> GenerateClaimsAsync(TUser user)
        {
            var claims = new List<Claim>();

            try
            {
                // Add standard claims
                claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.Value.ToString()));
                claims.Add(new Claim(ClaimTypes.Name, user.UserName ?? string.Empty));
                claims.Add(new Claim(ClaimTypes.Email, user.Email?.Address ?? string.Empty));

                // Add custom claims
                claims.Add(new Claim(ClaimTypes.GivenName, user.Profile.FirstName ?? string.Empty));

                claims.Add(new Claim(ClaimTypes.Surname, user.Profile.LastName));

                // Add phone number if available
                if (user.PhoneNumber != null)
                {
                    claims.Add(new Claim(ClaimTypes.MobilePhone, user.PhoneNumber.Number));
                }

                // Add email confirmation status
                claims.Add(new Claim("email_verified", user.Email?.IsVerified.ToString().ToLower() ?? string.Empty));

                // Add phone confirmation status
                claims.Add(new Claim("phone_verified", user.PhoneNumber?.IsVerified.ToString().ToLower() ?? string.Empty));

                // Add security stamp
                //todo: security stamp

                // Add user creation date
                claims.Add(new Claim("created_at", user.CreatedAt.ToString("O")));

                // Add user status
                claims.Add(new Claim("status", user.Status.Name.ToString().ToLower()));

                _logger.LogDebug("Generated {ClaimCount} claims for user {UserId}", claims.Count, user.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating claims for user {UserId}", user.Id);
                throw;
            }

            return claims;
        }
    }
} 