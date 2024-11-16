using System.Security.Claims;
using Bonyan.IdentityManagement.Domain;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.Layer.Application.Services;
using Bonyan.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Bonyan.IdentityManagement.Application;

public class BonAuthService<TUser> : BonApplicationService, IBonAuthService where TUser : class, IBonIdentityUser
{
    private IHttpContextAccessor HttpContextAccessor =>
        LazyServiceProvider.LazyGetRequiredService<IHttpContextAccessor>();

    private BonIdentityUserManager<TUser> IdentityUserManager => LazyServiceProvider.LazyGetRequiredService<BonIdentityUserManager<TUser>>();

    private ILogger<BonAuthService<TUser>> Logger =>
        LazyServiceProvider.LazyGetRequiredService<ILogger<BonAuthService<TUser>>>();


    /// <inheritdoc />
    public async Task<bool> LoginWithCookieAsync(string username, string password, bool isPersistent)
    {
        try
        {
            // Find the user by username
            var findResult = await IdentityUserManager.FindByUserNameAsync(username);
            if (findResult.IsFailure)
            {
                Logger.LogWarning($"Login failed: User '{username}' not found.");
                return false;
            }

            var user = findResult.Value;
            // Verify the password
            if ( !user.VerifyPassword(password))
            {
                Logger.LogWarning($"Login failed: Invalid password for user '{username}'.");
                return false;
            }

            // Get user roles
            var roles = new List<string>();
            foreach (var role in user.Roles)
            {
                roles.Add(role.Name);
            }

            // Create user claims
            var claims = new List<Claim>
            {
                new(BonClaimTypes.UserId, user.Id.Value.ToString()),
                new(BonClaimTypes.UserName, user.UserName),
                new(BonClaimTypes.Email, user.Email?.Address ?? string.Empty),
                new(BonClaimTypes.Email, user.PhoneNumber?.Number ?? string.Empty),
                new(BonClaimTypes.RememberMe, isPersistent.ToString()),
            };

            // Add role claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = isPersistent, // Assuming 'RememberMe' property exists
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1) // Set expiration as needed
            };

            // Sign in the user
            await HttpContextAccessor.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            Logger.LogInformation($"User '{username}' signed in successfully.");
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"An error occurred while signing in user '{username}'.");
            return false;
        }
    }
}