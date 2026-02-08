using Bonyan.Modularity;

namespace Bonyan.IdentityManagement.Options
{
    /// <summary>
    /// Configuration options for Bonyan Identity Management
    /// </summary>
    public class BonIdentityManagementOptions
    {
        /// <summary>
        /// Configuration context
        /// </summary>
        public BonPostConfigurationContext Context { get; }

        /// <summary>
        /// Whether to enable permission-based authorization
        /// </summary>
        public bool EnablePermissions { get; set; } = true;

        /// <summary>
        /// Whether to enable role-based authorization
        /// </summary>
        public bool EnableRoles { get; set; } = true;

        /// <summary>
        /// Whether to enable claims-based authorization
        /// </summary>
        public bool EnableClaims { get; set; } = true;

        /// <summary>
        /// Whether to enable hierarchical permissions
        /// </summary>
        public bool EnableHierarchicalPermissions { get; set; } = true;

        /// <summary>
        /// Whether to enable permission caching
        /// </summary>
        public bool EnablePermissionCaching { get; set; } = true;

        /// <summary>
        /// Permission cache duration
        /// </summary>
        public TimeSpan PermissionCacheDuration { get; set; } = TimeSpan.FromMinutes(30);

        /// <summary>
        /// Whether to enable user lockout
        /// </summary>
        public bool EnableUserLockout { get; set; } = true;

        /// <summary>
        /// Maximum failed login attempts before lockout
        /// </summary>
        public int MaxFailedLoginAttempts { get; set; } = 5;

        /// <summary>
        /// User lockout duration
        /// </summary>
        public TimeSpan UserLockoutDuration { get; set; } = TimeSpan.FromMinutes(15);

        /// <summary>
        /// Whether to enable password validation
        /// </summary>
        public bool EnablePasswordValidation { get; set; } = true;

        /// <summary>
        /// Minimum password length
        /// </summary>
        public int MinimumPasswordLength { get; set; } = 8;

        /// <summary>
        /// Whether to require uppercase letters in password
        /// </summary>
        public bool RequireUppercaseInPassword { get; set; } = true;

        /// <summary>
        /// Whether to require lowercase letters in password
        /// </summary>
        public bool RequireLowercaseInPassword { get; set; } = true;

        /// <summary>
        /// Whether to require digits in password
        /// </summary>
        public bool RequireDigitsInPassword { get; set; } = true;

        /// <summary>
        /// Whether to require special characters in password
        /// </summary>
        public bool RequireSpecialCharsInPassword { get; set; } = true;

        /// <summary>
        /// Whether to enable two-factor authentication
        /// </summary>
        public bool EnableTwoFactorAuthentication { get; set; } = false;

        /// <summary>
        /// Whether to enable email confirmation
        /// </summary>
        public bool EnableEmailConfirmation { get; set; } = false;

        /// <summary>
        /// Whether to enable phone number confirmation
        /// </summary>
        public bool EnablePhoneNumberConfirmation { get; set; } = false;

        /// <summary>
        /// JWT token expiration time
        /// </summary>
        public TimeSpan TokenExpiration { get; set; } = TimeSpan.FromHours(24);

        /// <summary>
        /// Refresh token expiration time
        /// </summary>
        public TimeSpan RefreshTokenExpiration { get; set; } = TimeSpan.FromDays(30);

        public BonIdentityManagementOptions(BonPostConfigurationContext context , CancellationToken cancellationToken = default)
        {
            Context = context;
        }
    }
} 