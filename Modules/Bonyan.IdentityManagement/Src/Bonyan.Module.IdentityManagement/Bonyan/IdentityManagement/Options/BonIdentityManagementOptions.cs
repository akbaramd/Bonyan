using Bonyan.Modularity;

namespace Bonyan.IdentityManagement.Options;

/// <summary>
/// Configuration options for Bonyan Identity Management. Use from host to configure
/// permissions, claims, lockout, password rules, tokens, and claim providers.
/// Other modules can add claim providers via DI (IBonIdentityClaimProvider) and
/// permission definitions via IBonPermissionDefinitionProvider.
/// </summary>
public class BonIdentityManagementOptions
{
    public BonPostConfigurationContext Context { get; }

    #region Permissions

    /// <summary>Enable permission-based authorization (bon.permission claims and policies).</summary>
    public bool EnablePermissions { get; set; } = true;

    /// <summary>Enable role-based authorization (ClaimTypes.Role).</summary>
    public bool EnableRoles { get; set; } = true;

    /// <summary>Enable claims-based authorization.</summary>
    public bool EnableClaims { get; set; } = true;

    /// <summary>Enable hierarchical permissions (parent implies child).</summary>
    public bool EnableHierarchicalPermissions { get; set; } = true;

    /// <summary>Enable permission result caching.</summary>
    public bool EnablePermissionCaching { get; set; } = true;

    /// <summary>Permission cache duration.</summary>
    public TimeSpan PermissionCacheDuration { get; set; } = TimeSpan.FromMinutes(30);

    #endregion

    #region Lockout

    /// <summary>Enable user lockout after failed attempts.</summary>
    public bool EnableUserLockout { get; set; } = true;

    /// <summary>Max failed login attempts before lockout.</summary>
    public int MaxFailedLoginAttempts { get; set; } = 5;

    /// <summary>Lockout duration.</summary>
    public TimeSpan UserLockoutDuration { get; set; } = TimeSpan.FromMinutes(15);

    #endregion

    #region Password

    /// <summary>Enable password complexity validation.</summary>
    public bool EnablePasswordValidation { get; set; } = true;

    public int MinimumPasswordLength { get; set; } = 8;
    public bool RequireUppercaseInPassword { get; set; } = true;
    public bool RequireLowercaseInPassword { get; set; } = true;
    public bool RequireDigitsInPassword { get; set; } = true;
    public bool RequireSpecialCharsInPassword { get; set; } = true;

    #endregion

    #region Confirmation & 2FA

    public bool EnableTwoFactorAuthentication { get; set; } = false;
    public bool EnableEmailConfirmation { get; set; } = false;
    public bool EnablePhoneNumberConfirmation { get; set; } = false;

    #endregion

    #region Tokens

    public TimeSpan TokenExpiration { get; set; } = TimeSpan.FromHours(24);
    public TimeSpan RefreshTokenExpiration { get; set; } = TimeSpan.FromDays(30);

    #endregion

    #region Cookie (when using Bonyan.AspNetCore.Authentication.Cookie)

    /// <summary>Login path for cookie authentication (e.g. /Account/Login). Host should pass this to ConfigureCookieAuthentication when using cookie auth.</summary>
    public string CookieLoginPath { get; set; } = "/Account/Login";

    /// <summary>Logout path for cookie authentication (e.g. /Account/Logout).</summary>
    public string CookieLogoutPath { get; set; } = "/Account/Logout";

    /// <summary>Access denied path for cookie authentication (e.g. /Account/AccessDenied).</summary>
    public string CookieAccessDeniedPath { get; set; } = "/Account/AccessDenied";

    /// <summary>Return URL query parameter name (e.g. returnUrl).</summary>
    public string CookieReturnUrlParameter { get; set; } = "returnUrl";

    /// <summary>Cookie expiration time span when using cookie authentication.</summary>
    public TimeSpan CookieExpireTimeSpan { get; set; } = TimeSpan.FromDays(14);

    /// <summary>Sliding expiration for cookie authentication.</summary>
    public bool CookieSlidingExpiration { get; set; } = true;

    #endregion

    #region Claim providers

    /// <summary>Include role names in the context passed to claim providers (for custom claims by role).</summary>
    public bool ClaimProviderIncludeRoleNames { get; set; } = true;

    /// <summary>Include permission names in the context passed to claim providers.</summary>
    public bool ClaimProviderIncludePermissionNames { get; set; } = true;

    /// <summary>Other modules add claim providers by registering IBonIdentityClaimProvider in DI (e.g. AddTransient&lt;IBonIdentityClaimProvider, MyProvider&gt;).</summary>

    #endregion

    public BonIdentityManagementOptions(BonPostConfigurationContext context, CancellationToken cancellationToken = default)
    {
        Context = context;
    }
} 