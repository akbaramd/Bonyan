namespace Bonyan.IdentityManagement
{
    public class BonJwtOptions
    {
        /// <summary>
        /// Indicates whether JWT authentication is enabled.
        /// </summary>
        public bool Enabled { get; set; } = false;

        /// <summary>
        /// The secret key used to sign JWT tokens. This is required.
        /// </summary>
        public string SecretKey { get; set; } = string.Empty;

        /// <summary>
        /// The issuer of the JWT tokens.
        /// </summary>
        public string Issuer { get; set; } = string.Empty;

        /// <summary>
        /// The audience of the JWT tokens.
        /// </summary>
        public string Audience { get; set; } = string.Empty;

        /// <summary>
        /// The expiration time (in minutes) for the JWT tokens. Defaults to 60 minutes.
        /// </summary>
        public int ExpirationInMinutes { get; set; } = 60;

        /// <summary>
        /// Indicates whether HTTPS metadata is required. Defaults to false.
        /// </summary>
        public bool RequireHttpsMetadata { get; set; } = false;

        /// <summary>
        /// Indicates whether the token should be saved. Defaults to true.
        /// </summary>
        public bool SaveToken { get; set; } = true;
    }
}
namespace Bonyan.IdentityManagement
{
    public class BonCookieOptions
    {
        /// <summary>
        /// The login path for cookie authentication. Defaults to "/Account/Login".
        /// </summary>
        public string LoginPath { get; set; } = "/Account/Login";

        /// <summary>
        /// The access denied path for unauthorized requests. Defaults to "/Account/AccessDenied".
        /// </summary>
        public string AccessDeniedPath { get; set; } = "/Account/AccessDenied";

        /// <summary>
        /// The logout path for cookie authentication. Defaults to "/Account/Logout".
        /// </summary>
        public string LogoutPath { get; set; } = "/Account/Logout";

        /// <summary>
        /// Indicates whether to issue a sliding expiration cookie. Defaults to true.
        /// </summary>
        public bool SlidingExpiration { get; set; } = true;

        /// <summary>
        /// The expiration time (in minutes) for the authentication cookie. Defaults to 60 minutes.
        /// </summary>
        public int ExpirationInMinutes { get; set; } = 60;

        /// <summary>
        /// The cookie name for authentication. Defaults to ".AspNetCore.Cookies".
        /// </summary>
        public string CookieName { get; set; } = ".AspNetCore.Cookies";

        /// <summary>
        /// Indicates whether the cookie is accessible only over HTTPS. Defaults to false.
        /// </summary>
        public bool CookieSecure { get; set; } = false;
    }
}

