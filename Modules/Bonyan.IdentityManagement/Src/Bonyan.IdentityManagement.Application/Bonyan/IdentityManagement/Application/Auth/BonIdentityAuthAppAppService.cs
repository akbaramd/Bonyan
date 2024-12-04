using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using AutoMapper;
using Bonyan.IdentityManagement.Application.Dto;
using Bonyan.IdentityManagement.Domain.Users;
using Bonyan.IdentityManagement.Domain.Users.DomainServices;
using Bonyan.IdentityManagement.Options;
using Bonyan.Layer.Application.Services;
using Bonyan.UserManagement.Domain.Users.ValueObjects;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Bonyan.IdentityManagement.Application.Auth;

public class BonIdentityAuthAppAppService<TUser> : BonApplicationService, IBonIdentityAuthAppService where TUser : class, IBonIdentityUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IBonIdentityUserManager<TUser> _identityUserManager;
    private readonly ILogger<BonIdentityAuthAppAppService<TUser>> _logger;
    private readonly BonAuthenticationJwtOptions _authenticationJwtOptions;
    private readonly IBonIdentityClaimProviderManager<TUser> _claimProviderManager;
    private readonly IMapper _mapper;

    public BonIdentityAuthAppAppService(
        IHttpContextAccessor httpContextAccessor,
        IBonIdentityUserManager<TUser> identityUserManager,
        ILogger<BonIdentityAuthAppAppService<TUser>> logger,
        BonAuthenticationJwtOptions jwtOptions,
        IBonIdentityClaimProviderManager<TUser> claimProviderManager,
        IMapper mapper)
    {
        _httpContextAccessor = httpContextAccessor;
        _identityUserManager = identityUserManager;
        _logger = logger;
        _authenticationJwtOptions = jwtOptions;
        _claimProviderManager = claimProviderManager;
        _mapper = mapper;
    }

    #region Registration

    public async Task<ServiceResult<bool>> RegisterAsync(BonIdentityUserRegistererDto createDto)
    {
        try
        {
            // Validate if username and email are unique
            var existingUser = await _identityUserManager.FindByUserNameAsync(createDto.UserName);
            if (existingUser.IsSuccess)
            {
                return ServiceResult<bool>.Failure("UserName already exists.");
            }

            var existingEmail = await _identityUserManager.FindByEmailAsync(createDto.Email);
            if (existingEmail.IsSuccess)
            {
                return ServiceResult<bool>.Failure("Email already exists.");
            }

            // Validate password complexity
            if (!IsValidPassword(createDto.Password))
            {
                return ServiceResult<bool>.Failure("Password does not meet complexity requirements.");
            }

            // Map DTO to User entity
            var newUser = _mapper.Map<TUser>(createDto);

            // Create the user in the system
            var result = await _identityUserManager.CreateAsync(newUser, createDto.Password);
            if (result.IsFailure)
            {
                return ServiceResult<bool>.Failure(result.ErrorMessage);
            }

            _logger.LogInformation($"User '{createDto.UserName}' registered successfully.");
            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while registering user '{createDto.UserName}'.");
            return ServiceResult<bool>.Failure(ex.Message);
        }
    }

    #endregion

    #region Profile

    public async Task<ServiceResult<BonIdentityUserDto>> ProfileAsync()
    {
        try
        {
            if (!BonCurrentUser.Id.HasValue)
            {
                return ServiceResult<BonIdentityUserDto>.Failure("User is not authenticated.");
            }

            var userId = BonUserId.FromValue(BonCurrentUser.Id.Value);

            var userResult = await _identityUserManager.FindByIdAsync(userId);
            if (userResult.IsFailure)
            {
                return ServiceResult<BonIdentityUserDto>.Failure("User not found.");
            }

            var user = userResult.Value;
            var rolesResult = await _identityUserManager.GetUserRolesAsync(user);
            if (rolesResult.IsFailure)
            {
                return ServiceResult<BonIdentityUserDto>.Failure("Failed to retrieve user roles.");
            }

            var userDto = _mapper.Map<BonIdentityUserDto>(user);

            return ServiceResult<BonIdentityUserDto>.Success(userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving user profile.");
            return ServiceResult<BonIdentityUserDto>.Failure(ex.Message);
        }
    }

    #endregion

    #region Token Handling

    private async Task<List<Claim>> GenerateClaimsAsync(TUser user)
    {
        var claims = new List<Claim>(await _claimProviderManager.GetClaimsAsync(user));

        return claims;
    }

    private string GenerateJwtToken(IEnumerable<Claim> claims)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationJwtOptions.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _authenticationJwtOptions.Issuer,
            audience: _authenticationJwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_authenticationJwtOptions.ExpirationInMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomBytes = new byte[32];
        using var rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
        rng.GetBytes(randomBytes);

        return Convert.ToBase64String(randomBytes);
    }

    #endregion

    #region Cookie Sign-In
    private async Task<TUser?> AuthenticateUserAsync(string username, string password)
    {
        var findResult = await _identityUserManager.FindByUserNameAsync(username);
        if (findResult.IsFailure)
        {
            _logger.LogWarning($"Login failed: User '{username}' not found.");
            return null;
        }

        var user = findResult.Value;

        if (!user.VerifyPassword(password))
        {
            _logger.LogWarning($"Login failed: Invalid password for user '{username}'.");
            return null;
        }

        return user;
    }
    public async Task<ServiceResult<bool>> CookieSignInAsync(string username, string password, bool isPersistent)
    {
        try
        {
            var user = await AuthenticateUserAsync(username, password);
            if (user == null)
                return ServiceResult<bool>.Failure("Invalid credentials.");

       

            var claims = await GenerateClaimsAsync(user);

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = isPersistent,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
            };

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

    #endregion

    #region JWT Sign-In

    public async Task<ServiceResult<BonIdentityJwtResultDto>> JwtBearerSignInAsync(string username, string password)
    {
        try
        {
            var user = await AuthenticateUserAsync(username, password);
            if (user == null)
                return ServiceResult<BonIdentityJwtResultDto>.Failure("Invalid credentials.");


            var claims = await GenerateClaimsAsync(user);
            var accessToken = GenerateJwtToken(claims);
            var refreshToken = GenerateRefreshToken();

            user.SetToken("RefreshToken", refreshToken, DateTime.UtcNow.AddDays(7));
            await _identityUserManager.UpdateAsync(user);

            return ServiceResult<BonIdentityJwtResultDto>.Success(new BonIdentityJwtResultDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Expired = TimeSpan.FromMinutes(_authenticationJwtOptions.ExpirationInMinutes)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during JWT sign-in.");
            return ServiceResult<BonIdentityJwtResultDto>.Failure(ex.Message);
        }
    }

    public async Task<ServiceResult<BonIdentityJwtResultDto>> RefreshTokenAsync(string refreshToken)
    {
        try
        {
            // Find the user by refresh token
            var userResult = await _identityUserManager.FindByTokenAsync("RefreshToken", refreshToken);
            if (userResult.IsFailure)
            {
                _logger.LogWarning("Refresh token is invalid or expired.");
                return ServiceResult<BonIdentityJwtResultDto>.Failure("Refresh token is invalid or expired.");
            }

            var user = userResult.Value;

            // Validate the refresh token (check if it exists and is not expired)
            var token = user.Tokens.FirstOrDefault(t => t.Type == "RefreshToken" && t.Value == refreshToken);
            if (token == null || token.IsExpired())
            {
                _logger.LogWarning("Refresh token is invalid or expired.");
                return ServiceResult<BonIdentityJwtResultDto>.Failure("Refresh token is invalid or expired.");
            }

            // Generate a new access token
            var claims = await GenerateClaimsAsync(user);
            var newAccessToken = GenerateJwtToken(claims);

            // Generate a new refresh token
            var newRefreshToken = GenerateRefreshToken();

            // Update the refresh token in the user's record
            user.SetToken("RefreshToken", newRefreshToken, DateTime.UtcNow.AddDays(7));
            await _identityUserManager.UpdateAsync(user);

            _logger.LogInformation($"Refresh token for user '{user.UserName}' has been successfully updated.");

            return ServiceResult<BonIdentityJwtResultDto>.Success(new BonIdentityJwtResultDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                Expired = TimeSpan.FromMinutes(_authenticationJwtOptions.ExpirationInMinutes)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while refreshing the token.");
            return ServiceResult<BonIdentityJwtResultDto>.Failure(ex.Message);
        }
    }

    #endregion

    #region Forgot and Reset Password

    public async Task<ServiceResult<string>> SendPasswordResetTokenAsync(string email)
    {
        try
        {
            var userResult = await _identityUserManager.FindByEmailAsync(email);
            if (userResult.IsFailure)
            {
                return ServiceResult<string>.Failure("User with the specified email was not found.");
            }

            var user = userResult.Value;
            var resetToken = GenerateRefreshToken();

            user.SetToken("PasswordReset", resetToken, DateTime.UtcNow.AddMinutes(15));
            await _identityUserManager.UpdateAsync(user);

            // Here, integrate with an email service to send the reset token to the user.

            _logger.LogInformation($"Password reset token sent to '{email}'.");
            return ServiceResult<string>.Success(resetToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while sending the password reset token.");
            return ServiceResult<string>.Failure(ex.Message);
        }
    }

    public async Task<ServiceResult<bool>> ResetPasswordAsync(string resetToken, string newPassword)
    {
        try
        {
            var userResult = await _identityUserManager.FindByTokenAsync("PasswordReset", resetToken);
            if (userResult.IsFailure)
            {
                return ServiceResult<bool>.Failure("Invalid or expired reset token.");
            }

            var user = userResult.Value;
            var token = user.Tokens.FirstOrDefault(t => t.Type == "PasswordReset" && t.Value == resetToken);

            if (token == null || token.IsExpired())
            {
                return ServiceResult<bool>.Failure("Invalid or expired reset token.");
            }

            var result = await _identityUserManager.ResetPasswordAsync(user, newPassword);
            if (result.IsFailure)
            {
                return ServiceResult<bool>.Failure(result.ErrorMessage);
            }

            user.RemoveToken("PasswordReset");
            await _identityUserManager.UpdateAsync(user);

            _logger.LogInformation($"Password for user '{user.UserName}' has been reset successfully.");
            return ServiceResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while resetting the password.");
            return ServiceResult<bool>.Failure(ex.Message);
        }
    }

    #endregion

    private bool IsValidPassword(string password)
    {
        // Example: check if the password meets basic criteria (length, number, special char)
        var regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");
        return regex.IsMatch(password);
    }
}