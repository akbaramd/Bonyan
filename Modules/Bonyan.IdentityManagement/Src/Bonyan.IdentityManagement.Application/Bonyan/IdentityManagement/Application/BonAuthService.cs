using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Bonyan.IdentityManagement.Application.Dto;
using Bonyan.IdentityManagement.Domain;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.IdentityManagement.Domain.Users.DomainServices;
using Bonyan.Layer.Application.Services;
using Bonyan.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Bonyan.IdentityManagement.Application;

public class BonAuthService<TUser> : BonApplicationService, IBonAuthService where TUser : class, IBonIdentityUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IBonIdentityUserManager<TUser> _identityUserManager;
    private readonly ILogger<BonAuthService<TUser>> _logger;
    private readonly BonJwtOptions _jwtOptions;

    public BonAuthService(
        IHttpContextAccessor httpContextAccessor,
        IBonIdentityUserManager<TUser> identityUserManager,
        ILogger<BonAuthService<TUser>> logger,
        IOptions<BonJwtOptions> jwtOptions)
    {
        _httpContextAccessor = httpContextAccessor;
        _identityUserManager = identityUserManager;
        _logger = logger;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<ServiceResult<bool>> CookieSignInAsync(string username, string password, bool isPersistent)
    {
        try
        {
            // Find the user by username
            var findResult = await _identityUserManager.FindByUserNameAsync(username);
            if (findResult.IsFailure)
            {
                _logger.LogWarning($"Login failed: User '{username}' not found.");
                return ServiceResult<bool>.Failure($"Login failed: User '{username}' not found.");
            }

            var user = findResult.Value;

            // Verify the password
            if (!user.VerifyPassword(password))
            {
                _logger.LogWarning($"Login failed: Invalid password for user '{username}'.");
                return ServiceResult<bool>.Failure($"Login failed: Invalid password for user '{username}'.");
            }

            // Retrieve roles using a role manager or service
            var rolesResult = await _identityUserManager.GetUserRolesAsync(user);
            if (rolesResult.IsFailure)
            {
                _logger.LogWarning($"Login failed: Could not retrieve roles for user '{username}'.");
                return ServiceResult<bool>.Failure($"Login failed: Could not retrieve roles for user '{username}'.");
            }

            var roles = rolesResult.Value;

            // Create user claims
            var claims = new List<Claim>
            {
                new(BonClaimTypes.UserId, user.Id.Value.ToString()),
                new(BonClaimTypes.UserName, user.UserName),
                new(BonClaimTypes.Email, user.Email?.Address ?? string.Empty),
                new(BonClaimTypes.PhoneNumber, user.PhoneNumber?.Number ?? string.Empty),
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
                IsPersistent = isPersistent,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
            };

            // Sign in the user
            await _httpContextAccessor.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            _logger.LogInformation($"User '{username}' signed in successfully.");
            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while signing in user '{username}'.");
            return ServiceResult<bool>.Failure(ex.Message);
        }
    }

    public async Task<ServiceResult<JwtResultDto>> JwtBearerSignInAsync(string username, string password)
    {
        try
        {
            // Find the user by username
            var findResult = await _identityUserManager.FindByUserNameAsync(username);
            if (findResult.IsFailure)
            {
                _logger.LogWarning($"Login failed: User '{username}' not found.");
                return ServiceResult<JwtResultDto>.Failure($"Login failed: User '{username}' not found.");
            }

            var user = findResult.Value;

            // Verify the password
            if (!user.VerifyPassword(password))
            {
                _logger.LogWarning($"Login failed: Invalid password for user '{username}'.");
                return ServiceResult<JwtResultDto>.Failure($"Login failed: Invalid password for user '{username}'.");
            }

            // Retrieve roles using a role manager or service
            var rolesResult = await _identityUserManager.GetUserRolesAsync(user);
            if (rolesResult.IsFailure)
            {
                _logger.LogWarning($"Login failed: Could not retrieve roles for user '{username}'.");
                return ServiceResult<JwtResultDto>.Failure(
                    $"Login failed: Could not retrieve roles for user '{username}'.");
            }

            var roles = rolesResult.Value;

            // Generate AccessToken
            var accessToken = GenerateJwtToken(user, roles);

            // Generate RefreshToken
            var refreshToken = GenerateRefreshToken();

            // Store RefreshToken securely (e.g., in database or cache)
            await StoreRefreshTokenAsync(user, refreshToken);

            _logger.LogInformation($"User '{username}' signed in successfully with JWT.");

            return ServiceResult<JwtResultDto>.Success(new JwtResultDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Expired = TimeSpan.FromMinutes(_jwtOptions.ExpirationInMinutes)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while signing in user '{username}' with JWT.");
            return ServiceResult<JwtResultDto>.Failure(ex.Message);
        }
    }

    private string GenerateRefreshToken()
    {
        var randomBytes = new byte[32];
        using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
        {
            rng.GetBytes(randomBytes);
        }

        return Convert.ToBase64String(randomBytes);
    }

    private async Task StoreRefreshTokenAsync(IBonIdentityUser user, string refreshToken)
    {
        // Example stub: Store in database or cache
        // await _identityUserManager.StoreRefreshTokenAsync(user.Id, refreshToken, DateTime.UtcNow.AddDays(7));

        // Log for demonstration
        _logger.LogInformation($"Stored refresh token for user {user.UserName}");
    }

    private string GenerateJwtToken(IBonIdentityUser user, IEnumerable<string> roles)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(BonClaimTypes.UserId, user.Id.Value.ToString()),
            new(BonClaimTypes.UserName, user.UserName),
            new(BonClaimTypes.Email, user.Email?.Address ?? string.Empty),
            new(BonClaimTypes.PhoneNumber, user.PhoneNumber?.Number ?? string.Empty)
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationInMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}